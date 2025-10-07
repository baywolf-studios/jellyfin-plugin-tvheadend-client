using System.Globalization;
using System.Reflection;
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

public class RecordingsChannel(
    ILogger<RecordingsChannel> logger,
    IServerApplicationHost appHost,
    IMediaEncoder mediaEncoder,
    ITvHeadendApiClient tvHeadendApiClient)
    : IChannel, IHasCacheKey, IRequiresMediaInfoCallback, IHasFolderAttributes, ISupportsDelete, ISupportsLatestMedia,
        ISupportsMediaProbe
{
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
        logger.LogInformation("GetChannelItems: Getting recording items");

        var result = new ChannelItemResult();
        var channelItems = new List<ChannelItemInfo>();

        try
        {
            var connectionInfo = Plugin.ConnectionInfo;
            var dvrConfigsTask = tvHeadendApiClient.GetDvrConfigGridAsync(connectionInfo,
                cancellationToken: cancellationToken);
            var upcomingDvrEventsTask = tvHeadendApiClient.GetDvrEventGridUpcomingAsync(
                connectionInfo,
                cancellationToken: cancellationToken);
            var finishedDvrEventsTask = tvHeadendApiClient.GetDvrEventGridFinishedAsync(
                connectionInfo,
                cancellationToken: cancellationToken);

            await Task.WhenAll(dvrConfigsTask, upcomingDvrEventsTask, finishedDvrEventsTask);

            var dvrConfigs =
                dvrConfigsTask.Result.Entries.ToDictionary(c => c.Uuid ?? Guid.NewGuid().ToString(), c => c);

            var allDvrEvents = upcomingDvrEventsTask.Result.Entries.Concat(finishedDvrEventsTask.Result.Entries);
            var playableDvrEvents = allDvrEvents.Where(r =>
                !string.IsNullOrEmpty(r.Filename) && !string.IsNullOrEmpty(r.Url) &&
                r.SchedStatus is "completed" or "recording");
            var dvrEventsWithData = playableDvrEvents.Select(dvrEvent =>
            {
                var uriPath = dvrEvent.Filename;
                if (!string.IsNullOrEmpty(dvrEvent.Filename)
                    && dvrConfigs.TryGetValue(dvrEvent.ConfigName ?? Guid.NewGuid().ToString(), out var dvrConfig)
                    && !string.IsNullOrEmpty(dvrConfig.Storage)
                    && dvrEvent.Filename.StartsWith(dvrConfig.Storage))
                {
                    uriPath = dvrEvent.Filename.Substring(dvrConfig.Storage.Length - 1);
                }

#pragma warning disable CS8604 // Possible null reference argument.
                return (Uri: new Uri(uriPath), DvrEvent: dvrEvent);
#pragma warning restore CS8604 // Possible null reference argument.
            }).ToList();

            var currentFolderDepth = 1;
            var currentFolderPathPrefix = string.Empty;
            if (!string.IsNullOrEmpty(query.FolderId))
            {
                var queryFolderUri = new Uri(Uri.UnescapeDataString(query.FolderId));
                currentFolderDepth = queryFolderUri.Segments.Length;
                currentFolderPathPrefix = query.FolderId;
            }

            var itemSegmentDepth = currentFolderDepth + 1;

            var addedFolderIds = new HashSet<string>();

            foreach (var dvrEvent in dvrEventsWithData)
            {
                if (!dvrEvent.Uri.AbsolutePath.StartsWith(currentFolderPathPrefix))
                {
                    continue;
                }

                if (dvrEvent.Uri.Segments.Length == itemSegmentDepth)
                {
                    channelItems.Add(ConvertToChannelItem(dvrEvent.DvrEvent));
                }
                else if (dvrEvent.Uri.Segments.Length > itemSegmentDepth)
                {
                    var nextFolderSegments = dvrEvent.Uri.Segments.Take(itemSegmentDepth);
                    var nextFolderUriPath = string.Concat(nextFolderSegments);
                    var decodedFolderUriPath = Uri.UnescapeDataString(nextFolderUriPath);
                    var nextFolderUri = new Uri(decodedFolderUriPath);
                    var nextFolderId = nextFolderUri.AbsolutePath;
                    var nextFolderTitle = Uri.UnescapeDataString(nextFolderUri.Segments.Last()).TrimEnd('/');

                    if (addedFolderIds.Add(nextFolderId))
                    {
                        var imageInfo = ImageUtilities.GetImageInfo(dvrEvent.DvrEvent.Image, appHost);
                        channelItems.Add(new ChannelItemInfo
                        {
                            Name = nextFolderTitle,
                            FolderType = ChannelFolderType.Container,
                            Id = nextFolderId,
                            Type = ChannelItemType.Folder,
                            ImageUrl = imageInfo.ImageUrl
                        });
                    }
                }
            }

            result.Items = channelItems;
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
            return Task.FromResult(new DynamicImageResponse
            {
                Path = "https://raw.githubusercontent.com/baywolf-studios/jellyfin-plugin-tvheadend-client/main/images/recordings-channel.png",
                Protocol = MediaProtocol.Http,
                HasImage = true
            });
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
        return string.Join('-', PluginInfo.Name, userId, DateTime.UtcNow.Ticks.ToString(CultureInfo.InvariantCulture));
    }

    public string[] Attributes => ["Recordings"];

    public async Task<IEnumerable<MediaSourceInfo>> GetChannelItemMediaInfo(string id,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetChannelItemMediaInfo: Generating MediaInfo for {Id}", id);

        var playbackUrl =
            await tvHeadendApiClient.GetPlayUrlForDvrFile(Plugin.ConnectionInfo, id,
                cancellationToken);
        var mediaSourceInfo =
                new MediaSourceInfo
                {
                    Id = id,
                    Path = playbackUrl,
                    Protocol = MediaProtocol.Http,
                    IsRemote = true,
                    AnalyzeDurationMs = 500,
                    BufferMs = 500,
                    FallbackMaxStreamingBitrate = 30000000,
                    UseMostCompatibleTranscodingProfile = true,
                    MediaStreams =
                    [
                        new MediaStream { Type = MediaStreamType.Video, Index = -1, IsInterlaced = true },
                        new MediaStream { Type = MediaStreamType.Audio, Index = -1 }
                    ]
                }
            ;

        try
        {
            var calculatedMediaSourceInfo = await mediaEncoder.GetMediaInfo(
                new MediaInfoRequest { ExtractChapters = false, MediaSource = mediaSourceInfo, MediaType = DlnaProfileType.Video },
                cancellationToken);
            mediaSourceInfo.MediaStreams = calculatedMediaSourceInfo.MediaStreams;
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
        logger.LogInformation("GetLatestMedia: Getting latest recording items");

        try
        {
            var connectionInfo = Plugin.ConnectionInfo;
            var upcomingRecordingsTask = tvHeadendApiClient.GetDvrEventGridUpcomingAsync(connectionInfo, 50,
                cancellationToken);
            var finishedRecordingsTask = tvHeadendApiClient.GetDvrEventGridFinishedAsync(
                connectionInfo,
                50,
                cancellationToken);

            await Task.WhenAll(upcomingRecordingsTask, finishedRecordingsTask);

            var allRecordings = upcomingRecordingsTask.Result.Entries.Concat(finishedRecordingsTask.Result.Entries);
            var playableRecordings = allRecordings.Where(r =>
                !string.IsNullOrEmpty(r.Url) && r.SchedStatus is "completed" or "recording");
            var latestRecordings = playableRecordings.OrderByDescending(r => r.StopDateTime);
            return latestRecordings.Select(ConvertToChannelItem);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetChannelItems: Failed to get channel items");
            throw;
        }
    }

    private ChannelItemInfo ConvertToChannelItem(DvrEventEntry dvrEventEntry)
    {
        if (string.IsNullOrEmpty(dvrEventEntry.Uuid) || string.IsNullOrEmpty(dvrEventEntry.Url))
        {
            return new ChannelItemInfo();
        }

        logger.LogInformation("ConvertToChannelItem: {id}", dvrEventEntry.Uuid);

        var isCurrentlyRecording = dvrEventEntry.SchedStatus == "recording";
        var imageInfo = ImageUtilities.GetImageInfo(dvrEventEntry.Image, appHost);
        var channelItem = new ChannelItemInfo
        {
            Name = string.IsNullOrEmpty(dvrEventEntry.Subtitle) ? dvrEventEntry.Title : dvrEventEntry.Subtitle,
            SeriesName = string.IsNullOrEmpty(dvrEventEntry.Subtitle) ? null : dvrEventEntry.Title,
            Id = dvrEventEntry.Uuid,
            DateModified = dvrEventEntry.StopDateTime ?? DateTime.UtcNow,
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
            ContentType = ChannelMediaContentType.Clip,
            //ExtraType
            //TrailerTypes
            //ProviderIds
            PremiereDate = dvrEventEntry.FirstAiredDateTime,
            ProductionYear = dvrEventEntry.CopyrightYear,
            DateCreated = dvrEventEntry.StartDateTime,
            StartDate = dvrEventEntry.StartDateTime,
            EndDate = dvrEventEntry.StopDateTime,
            //IndexNumber
            //ParentIndexNumber
            //MediaSources = [],
            //HomePageUrl
            //Artists
            //AlbumArtists
            IsLiveStream = isCurrentlyRecording
            //Etag
        };

        return channelItem;
    }
}
