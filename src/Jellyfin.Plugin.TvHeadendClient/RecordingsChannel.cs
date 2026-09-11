using System.Globalization;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.TvHeadendClient.Helpers;
using Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient;
using Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.MediaEncoding;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Channels;
using MediaBrowser.Model.Dlna;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TvHeadendClient;

public partial class RecordingsChannel(
    ILogger<RecordingsChannel> logger,
    IServerApplicationHost appHost,
    IMediaEncoder mediaEncoder,
    ITvHeadendApiClient tvHeadendApiClient)
    : IChannel, IHasCacheKey, IRequiresMediaInfoCallback, IHasFolderAttributes, ISupportsDelete, ISupportsLatestMedia
{
    private static readonly HashSet<string> _playableStatuses =
    [
        "completed",
        "completedWarning",
        "completedRerecord",
        "completedError",
        "recording"
    ];

    public InternalChannelFeatures GetChannelFeatures()
    {
        return new InternalChannelFeatures
        {
            ContentTypes =
            [
                ChannelMediaContentType.Movie,
                ChannelMediaContentType.Episode,
                ChannelMediaContentType.Clip
            ],
            MediaTypes =
            [
                ChannelMediaType.Video
            ],
            SupportsContentDownloading = true
        };
    }

    public bool IsEnabledFor(string userId)
    {
        return !Plugin.Instance.Configuration.HideRecordingsChannel;
    }

    public async Task<ChannelItemResult> GetChannelItems(InternalChannelItemQuery query,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("GetChannelItems: Getting recording items");

        var result = new ChannelItemResult();

        try
        {
            var connectionInfo = Plugin.ConnectionInfo;
            var upcomingDvrEventsTask = tvHeadendApiClient.GetDvrEventGridUpcomingAsync(
                connectionInfo,
                cancellationToken: cancellationToken);
            var finishedDvrEventsTask = tvHeadendApiClient.GetDvrEventGridFinishedAsync(
                connectionInfo,
                cancellationToken: cancellationToken);

            await Task.WhenAll(upcomingDvrEventsTask, finishedDvrEventsTask);

            var allDvrEvents = upcomingDvrEventsTask.Result.Entries.Concat(finishedDvrEventsTask.Result.Entries).ToList();

            result.Items = BuildChannelItems(allDvrEvents, query.FolderId);

            logger.LogDebug("GetChannelItems: Retrieved {Count} items", result.Items.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetChannelItems: Failed to get channel items");
            throw;
        }

        return result;
    }

    public Task<DynamicImageResponse> GetChannelImage(ImageType type, CancellationToken cancellationToken)
    {
        if (type == ImageType.Primary)
        {
            return Task.FromResult(new DynamicImageResponse { Path = "https://raw.githubusercontent.com/baywolf-studios/jellyfin-plugin-tvheadend-client/main/images/recordings-channel.png", Protocol = MediaProtocol.Http, HasImage = true });
        }

        return Task.FromResult(new DynamicImageResponse { HasImage = false });
    }

    public IEnumerable<ImageType> GetSupportedChannelImages()
    {
        return [ImageType.Primary];
    }

    public string Name => "Recordings";
    public string Description => "TvHeadend Recordings";
    public string DataVersion => PluginInfo.Version;
    public string HomePageUrl => PluginInfo.HomePageUrl;
    public ChannelParentalRating ParentalRating => ChannelParentalRating.GeneralAudience;

    public string GetCacheKey(string? userId)
    {
        return string.Join('-', PluginInfo.Name, userId, DateTime.UtcNow.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture));
    }

    public string[] Attributes => ["Recordings"];

    public async Task<IEnumerable<MediaSourceInfo>> GetChannelItemMediaInfo(string id,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("GetChannelItemMediaInfo: Generating MediaInfo for {Id}", id);

        var playbackUrl =
            await tvHeadendApiClient.GetPlayUrlForDvrFile(Plugin.ConnectionInfo, id,
                cancellationToken);
        var mediaSourceInfo =
                new MediaSourceInfo
                {
                    Id = id,
                    Path = playbackUrl,
                    Protocol = MediaProtocol.Http,
                    AnalyzeDurationMs = 5000,
                    FallbackMaxStreamingBitrate = 30_000_000,
                    UseMostCompatibleTranscodingProfile = !Plugin.Instance.Configuration.AllowFmp4TranscodingContainer,
                    MediaStreams =
                    [
                        new MediaStream { Type = MediaStreamType.Video, Index = -1, IsInterlaced = true },
                        new MediaStream { Type = MediaStreamType.Audio, Index = -1 }
                    ],
                    IgnoreDts = true
                }
            ;

        try
        {
            mediaSourceInfo = await mediaEncoder.GetMediaInfo(
                new MediaInfoRequest { ExtractChapters = false, MediaSource = mediaSourceInfo, MediaType = DlnaProfileType.Video },
                cancellationToken);

            mediaSourceInfo.Id = id;
            mediaSourceInfo.Path = playbackUrl;
            mediaSourceInfo.Protocol = MediaProtocol.Http;
            mediaSourceInfo.AnalyzeDurationMs = 5000;
            mediaSourceInfo.FallbackMaxStreamingBitrate = 30000000;
            mediaSourceInfo.UseMostCompatibleTranscodingProfile = !Plugin.Instance.Configuration.AllowFmp4TranscodingContainer;
            mediaSourceInfo.IgnoreDts = true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "GetChannelItemMediaInfo: Failed to probe media streams for {Id} using fallback", id);
        }

        return [mediaSourceInfo];
    }

    public bool CanDelete(BaseItem item)
    {
        return !item.IsFolder;
    }

    public async Task DeleteItem(string id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogWarning("DeleteItem: Id is null or empty");
            throw new ArgumentException("DeleteItem Id cannot be null or empty", nameof(id));
        }

        logger.LogDebug("DeleteItem: Deleting recording for id {Id}", id);

        try
        {
            await tvHeadendApiClient
                .RemoveDvrEntryAsync(Plugin.ConnectionInfo, id, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteItem: Unexpected error deleting recording");
        }
    }

    public async Task<IEnumerable<ChannelItemInfo>> GetLatestMedia(ChannelLatestMediaSearch request,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("GetLatestMedia: Getting latest recording items");

        try
        {
            var connectionInfo = Plugin.ConnectionInfo;
            var upcomingRecordingsTask = tvHeadendApiClient.GetDvrEventGridUpcomingAsync(connectionInfo, 50,
                cancellationToken);
            var finishedRecordingsTask = tvHeadendApiClient.GetDvrEventGridFinishedAsync(
                connectionInfo,
                cancellationToken: cancellationToken);

            await Task.WhenAll(upcomingRecordingsTask, finishedRecordingsTask);

            var allRecordings = upcomingRecordingsTask.Result.Entries.Concat(finishedRecordingsTask.Result.Entries).ToList();

            var items = BuildChannelItems(allRecordings, "latest");

            logger.LogDebug("GetLatestMedia: Retrieved {Count} playable recordings", items.Count);

            return items.OrderByDescending(r => r.EndDate);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetLatestMedia: Failed to get latest recording items");
            throw;
        }
    }

    private List<ChannelItemInfo> BuildChannelItems(IEnumerable<DvrEventEntry> dvrEventEntries, string? folderId)
    {
        var channelItems = dvrEventEntries
            .Where(r =>
                !string.IsNullOrEmpty(r.Filename) &&
                !string.IsNullOrEmpty(r.Url) &&
                _playableStatuses.Contains(r.SchedStatus ?? string.Empty))
            .Select(dvrEventEntry =>
            {
                var (season, episode) = GetSeasonEpisodeInfo(dvrEventEntry);
                return (
                    Recording: dvrEventEntry,
                    Season: season,
                    Episode: episode
                );
            })
            .ToList();

        if (folderId?.StartsWith("recordings:", StringComparison.Ordinal) == true)
        {
            var title = Uri.UnescapeDataString(folderId["recordings:".Length..]);

            return channelItems
                .Where(e => e.Recording.Title == title)
                .Select(e => ConvertToChannelItem(
                    e.Recording,
                    ChannelMediaContentType.Episode,
                    e.Season,
                    e.Episode))
                .ToList();
        }

        if (folderId?.StartsWith("season:", StringComparison.Ordinal) == true)
        {
            var parts = folderId.Split(':', 3);

            var seriesName = Uri.UnescapeDataString(parts[1]);
            var season = int.Parse(parts[2], CultureInfo.InvariantCulture);

            return channelItems
                .Where(e => e.Recording.Title == seriesName && (e.Season ?? 0) == season)
                .Select(e => ConvertToChannelItem(e.Recording, ChannelMediaContentType.Episode, e.Season ?? 0, e.Episode))
                .ToList();
        }

        if (folderId?.StartsWith("series:", StringComparison.Ordinal) == true)
        {
            var seriesName = Uri.UnescapeDataString(folderId["series:".Length..]);

            return channelItems
                .Where(e => e.Recording.Title == seriesName)
                .GroupBy(item => item.Season ?? 0)
                .Select(g =>
                    {
                        var season = g.Key;
                        return new ChannelItemInfo
                        {
                            Name = $"Season {season}",
                            Id = $"season:{Uri.EscapeDataString(seriesName)}:{season}",
                            Type = ChannelItemType.Folder,
                            FolderType = ChannelFolderType.Season,
                            ContentType = ChannelMediaContentType.Episode,
                            ImageUrl = ImageUtilities
                                .GetImageInfo(g.First().Recording.Image, appHost)
                                .ImageUrl
                        };
                    }
                )
                .ToList();
        }

        return channelItems
            .GroupBy(channelItem => channelItem.Recording.Title ?? string.Empty)
            .SelectMany(group =>
            {
                var items = group.ToList();

                var isMovie =
                    items.Count == 1 &&
                    string.IsNullOrWhiteSpace(items[0].Recording.Subtitle) &&
                    !items[0].Season.HasValue &&
                    !items[0].Episode.HasValue;

                if (isMovie)
                {
                    return
                    [
                        ConvertToChannelItem(
                            items[0].Recording,
                            ChannelMediaContentType.Movie,
                            null,
                            null)
                    ];
                }

                if (folderId == "latest")
                {
                    return items.Select(item =>
                        ConvertToChannelItem(
                            item.Recording,
                            ChannelMediaContentType.Episode,
                            item.Season,
                            item.Episode));
                }

                var hasSeasons = items.Any(item => item.Season.HasValue);

                return
                [
                    new ChannelItemInfo
                    {
                        Name = group.Key,
                        Id = hasSeasons
                            ? $"series:{Uri.EscapeDataString(group.Key)}"
                            : $"recordings:{Uri.EscapeDataString(group.Key)}",
                        Type = ChannelItemType.Folder,
                        FolderType = hasSeasons
                            ? ChannelFolderType.Series
                            : ChannelFolderType.Container,
                        ImageUrl = ImageUtilities
                            .GetImageInfo(items[0].Recording.Image, appHost)
                            .ImageUrl
                    }
                ];
            })
            .ToList();
    }

    private ChannelItemInfo ConvertToChannelItem(DvrEventEntry dvrEventEntry, ChannelMediaContentType contentType, int? season, int? episode)
    {
        if (string.IsNullOrEmpty(dvrEventEntry.Uuid) || string.IsNullOrEmpty(dvrEventEntry.Url))
        {
            return new ChannelItemInfo();
        }

        logger.LogDebug("ConvertToChannelItem: {id}", dvrEventEntry.Uuid);

        var isCurrentlyRecording = dvrEventEntry.SchedStatus == "recording";
        var imageInfo = ImageUtilities.GetImageInfo(dvrEventEntry.Image, appHost);
        var channelItem = new ChannelItemInfo
        {
            Name = contentType == ChannelMediaContentType.Episode
                ? string.IsNullOrWhiteSpace(dvrEventEntry.Subtitle)
                    ? dvrEventEntry.StartDateTime?.ToLocalTime().ToString("g", CultureInfo.CurrentCulture)
                    : dvrEventEntry.Subtitle
                : dvrEventEntry.Title,
            SeriesName = contentType == ChannelMediaContentType.Episode ? dvrEventEntry.Title : null,
            Id = dvrEventEntry.Uuid,
            DateModified = isCurrentlyRecording ? DateTime.UtcNow : dvrEventEntry.StopRealDateTime ?? DateTime.UtcNow,
            Type = ChannelItemType.Media,
            OfficialRating = dvrEventEntry.RatingLabel,
            Overview = dvrEventEntry.Description,
            //Genres
            //Studios
            //Tags
            //People
            //CommunityRating
            RunTimeTicks = dvrEventEntry.Duration == null || isCurrentlyRecording
                ? null
                : TimeSpan.FromSeconds((double)dvrEventEntry.Duration).Ticks,
            ImageUrl = imageInfo.ImageUrl,
            //OriginalTitle
            MediaType = ChannelMediaType.Video,
            //FolderType
            ContentType = contentType,
            //ExtraType
            //TrailerTypes
            //ProviderIds
            PremiereDate = dvrEventEntry.FirstAiredDateTime,
            ProductionYear = dvrEventEntry.CopyrightYear,
            DateCreated = dvrEventEntry.StartRealDateTime,
            StartDate = dvrEventEntry.StartRealDateTime,
            EndDate = dvrEventEntry.StopRealDateTime,
            IndexNumber = episode,
            ParentIndexNumber = season,
            //MediaSources = [],
            //HomePageUrl
            //Artists
            //AlbumArtists
            IsLiveStream = isCurrentlyRecording
            //Etag
        };

        return channelItem;
    }

    private static (int? Season, int? Episode) GetSeasonEpisodeInfo(DvrEventEntry item)
    {
        var match = SeasonEpisodeRegex().Match($"{item.EpisodeDisplay} {item.Filename}");

        if (!match.Success)
        {
            return (null, null);
        }

        return (
            int.Parse(match.Groups[1].Success
                ? match.Groups[1].Value
                : match.Groups[3].Value),
            int.Parse(match.Groups[2].Success
                ? match.Groups[2].Value
                : match.Groups[4].Value)
        );
    }

    [GeneratedRegex(@"(?:Season\s*(\d+)\.Episode\s*(\d+)|S(\d+)E(\d+))", RegexOptions.IgnoreCase)]
    private static partial Regex SeasonEpisodeRegex();
}
