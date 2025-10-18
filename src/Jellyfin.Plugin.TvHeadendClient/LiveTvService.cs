using Jellyfin.Plugin.TvHeadendClient.Helpers;
using Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient;
using Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;
using MediaBrowser.Controller;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.MediaInfo;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.TvHeadendClient;

public class LiveTvService(
    ILogger<LiveTvService> logger,
    IServerApplicationHost appHost,
    ITvHeadendApiClient tvHeadendApiClient)
    : ILiveTvService, ISupportsNewTimerIds
{
    public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(CancellationToken cancellationToken)
    {
        var channels = new List<ChannelInfo>();

        logger.LogDebug("GetChannels: Requesting channel list");

        try
        {
            var response = await tvHeadendApiClient
                .GetChannelGridAsync(Plugin.ConnectionInfo, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            foreach (var entry in response.Entries)
            {
                var imageInfo = ImageUtilities.GetImageInfo(entry.IconPublicUrl, appHost);
                channels.Add(new ChannelInfo
                {
                    Id = entry.Uuid,
                    Name = entry.Name,
                    Number = entry.Number,
                    HasImage = imageInfo.HasImage,
                    ImageUrl = imageInfo.ImageUrl
                });
            }

            logger.LogInformation("GetChannels: Retrieved {Count} channels.", channels.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetChannels: Unexpected error retrieving channels");
        }

        return channels;
    }

    public async Task CancelTimerAsync(string timerId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(timerId))
        {
            logger.LogWarning("CancelTimer: TimerId is null or empty");
            throw new ArgumentException("CancelTimer TimerId cannot be null or empty", nameof(timerId));
        }

        logger.LogDebug("CancelTimer: Cancelling timer for timerId {TimerId}", timerId);

        try
        {
            await tvHeadendApiClient
                .CancelDvrEntryAsync(Plugin.ConnectionInfo, timerId, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CancelTimer: Unexpected error cancelling timer");
        }
    }

    public async Task CancelSeriesTimerAsync(string timerId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(timerId))
        {
            logger.LogWarning("CancelSeriesTimer: TimerId is null or empty");
            throw new ArgumentException("CancelSeriesTimer TimerId cannot be null or empty", nameof(timerId));
        }

        logger.LogDebug("CancelSeriesTimer: Cancelling series timer for timerId {TimerId}", timerId);

        try
        {
            await tvHeadendApiClient
                .CancelDvrAutorecAsync(Plugin.ConnectionInfo, timerId, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CancelSeriesTimer: Unexpected error cancelling series timer");
        }
    }

    public async Task CreateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
    {
        await CreateTimer(info, cancellationToken).ConfigureAwait(false);
    }

    public async Task CreateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
    {
        await CreateSeriesTimer(info, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateTimerAsync(TimerInfo updatedTimer, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(updatedTimer.Id))
        {
            logger.LogWarning("UpdateTimer: Timer ID is null or empty");
            throw new ArgumentException("Timer ID cannot be null or empty", nameof(updatedTimer));
        }

        logger.LogDebug("UpdateTimer: Updating for timerId {TimerId}", updatedTimer.Id);

        try
        {
            await tvHeadendApiClient.UpdateDvrEntryAsync(Plugin.ConnectionInfo,
                new DvrEntryUpdateRequest
                {
                    Uuid = updatedTimer.Id,
                    ChannelId = updatedTimer.ChannelId,
                    Title = updatedTimer.Name,
                    ExtraText = updatedTimer.Overview,
                    Start = new DateTimeOffset(updatedTimer.StartDate).ToUnixTimeSeconds(),
                    Stop = new DateTimeOffset(updatedTimer.EndDate).ToUnixTimeSeconds(),
                    StartExtra = (int)Math.Round(updatedTimer.PrePaddingSeconds / 60.0),
                    StopExtra = (int)Math.Round(updatedTimer.PostPaddingSeconds / 60.0)
                }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateTimer: Failed to update timer for timerId {TimerId}", updatedTimer.Id);
            throw;
        }
    }

    public async Task UpdateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(info.Id))
        {
            logger.LogWarning("UpdateSeriesTimer: Timer ID is null or empty");
            throw new ArgumentException("Timer ID cannot be null or empty", nameof(info));
        }

        logger.LogDebug("UpdateSeriesTimer: Updating for timerId {TimerId}", info.Id);

        try
        {
            var request = new DvrAutorecUpdateRequest
            {
                Uuid = info.Id,
                ChannelId = info.RecordAnyChannel ? string.Empty : info.ChannelId,
                Name = info.Name,
                Title = info.Name,
                BType = info.RecordNewOnly ? 1 : 0,
                MaxCount = info.KeepUpTo,
                Record = info.SkipEpisodesInLibrary ? 14 : 0,
                StartExtra = (int)Math.Round(info.PrePaddingSeconds / 60.0),
                StopExtra = (int)Math.Round(info.PostPaddingSeconds / 60.0)
            };
            request.Weekdays.AddRange([1, 2, 3, 4, 5, 6, 7]);

            await tvHeadendApiClient.UpdateDvrAutorecAsync(Plugin.ConnectionInfo,
                request, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "UpdateSeriesTimer: Failed to update series timer for timerId {TimerId}", info.Id);
            throw;
        }
    }

    public async Task<IEnumerable<TimerInfo>> GetTimersAsync(CancellationToken cancellationToken)
    {
        var timers = new List<TimerInfo>();

        logger.LogDebug("GetTimers: Requesting timer list");

        try
        {
            var response = await tvHeadendApiClient
                .GetDvrEventGridAsync(Plugin.ConnectionInfo, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            foreach (var entry in response.Entries)
            {
                timers.Add(new TimerInfo
                {
                    // ProviderIds
                    // SeriesProviderIds
                    // Tags
                    Id = entry.Uuid,
                    SeriesTimerId = entry.Autorec,
                    ChannelId = entry.Channel,
                    ProgramId = entry.Broadcast.ToString(),
                    ShowId = entry.Parent,
                    Name = entry.Title,
                    Overview = entry.Description,
                    // SeriesId
                    StartDate = entry.StartRealDateTime ?? DateTime.UtcNow,
                    EndDate = entry.StopRealDateTime ?? DateTime.UtcNow.AddHours(1),
                    Status = entry.SchedStatus switch
                    {
                        "scheduled" => RecordingStatus.New,
                        "recording" => RecordingStatus.InProgress,
                        "completed" => RecordingStatus.Completed,
                        "completedError" => RecordingStatus.Error,
                        "cancelled" => RecordingStatus.Cancelled,
                        "conflictedOk" => RecordingStatus.ConflictedOk,
                        "conflictedNotOk" => RecordingStatus.ConflictedNotOk,
                        _ => RecordingStatus.Error
                    },
                    PrePaddingSeconds = entry.StartExtra * 60 ?? 0,
                    PostPaddingSeconds = entry.StopExtra * 60 ?? 0,
                    // IsPrePaddingRequired
                    // IsPostPaddingRequired,
                    // IsManual
                    Priority = entry.Priority ?? 0,
                    // RetryCount
                    // SeasonNumber
                    // EpisodeNumber
                    // IsMovie
                    // IsKids => derived from Tags
                    // IsSports => derived from Tags
                    // IsNews => derived from Tags
                    // IsSeries
                    // IsLive => derived from Tags
                    // IsPremiere => derived from Tags
                    ProductionYear = entry.CopyrightYear,
                    EpisodeTitle = entry.Subtitle,
                    OriginalAirDate = entry.FirstAiredDateTime,
                    // IsProgramSeries
                    IsRepeat = entry.Duplicate ?? false,
                    // HomePageUrl
                    // CommunityRating
                    OfficialRating = entry.RatingLabel,
                    // Genres
                    RecordingPath = entry.Filename
                    // KeepUntil
                });
            }

            logger.LogInformation("GetTimers: Retrieved {Count} timers", timers.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetTimers: Unexpected error retrieving timers");
        }

        return timers;
    }

    public async Task<SeriesTimerInfo> GetNewTimerDefaultsAsync(CancellationToken cancellationToken,
        ProgramInfo? program)
    {
        var defaultTimer = new SeriesTimerInfo();

        logger.LogDebug("GetNewTimerDefaults: Fetching recording profiles");

        try
        {
            var response = await tvHeadendApiClient
                .GetDvrConfigGridAsync(Plugin.ConnectionInfo, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var recordingProfiles = response.Entries;

            var defaultRecordingProfile =
                recordingProfiles.FirstOrDefault(r => r.Name == string.Empty && (r.Enabled ?? false)) ??
                recordingProfiles.FirstOrDefault(r => r.Enabled ?? false);

            if (defaultRecordingProfile != null)
            {
                defaultTimer.Id = defaultRecordingProfile.Uuid;
                defaultTimer.ChannelId = program?.ChannelId;
                defaultTimer.ProgramId = program?.Id;
                defaultTimer.Name = program?.Name;
                defaultTimer.ServiceName = Name;
                defaultTimer.Overview = program?.Overview;
                defaultTimer.StartDate = program?.StartDate ?? DateTime.UtcNow;
                defaultTimer.EndDate = program?.EndDate ?? DateTime.UtcNow.AddHours(1);
                defaultTimer.RecordAnyTime = true;
                defaultTimer.RecordAnyChannel = false;
                defaultTimer.KeepUpTo = Math.Clamp(defaultRecordingProfile.AutorecMaxcount ?? 0, 0, 50);
                // KeepUntil
                defaultTimer.SkipEpisodesInLibrary = (defaultRecordingProfile.Record ?? 0) != 0;
                defaultTimer.RecordNewOnly = false;
                defaultTimer.Days = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
                defaultTimer.Priority = defaultRecordingProfile.Priority ?? 0;
                defaultTimer.PrePaddingSeconds = (defaultRecordingProfile.PreExtraTime ?? 0) * 60;
                defaultTimer.PostPaddingSeconds = (defaultRecordingProfile.PostExtraTime ?? 0) * 60;
                // IsPrePaddingRequired
                // IsPostPaddingRequired
                defaultTimer.SeriesId = program?.SeriesId;

                logger.LogDebug("GetNewTimerDefaults: Default recording profile applied with ID {Id}",
                    defaultTimer.Id);
            }
            else
            {
                logger.LogWarning("GetNewTimerDefaults: No enabled recording profile found in response");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetNewTimerDefaults: Unexpected error");
        }

        return defaultTimer;
    }

    public async Task<IEnumerable<SeriesTimerInfo>> GetSeriesTimersAsync(CancellationToken cancellationToken)
    {
        var timers = new List<SeriesTimerInfo>();

        try
        {
            var response = await tvHeadendApiClient
                .GetDvrAutorecGridAsync(Plugin.ConnectionInfo, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            foreach (var entry in response.Entries)
            {
                var channelId = entry.Channel;

                timers.Add(new SeriesTimerInfo
                {
                    Id = entry.Uuid,
                    ChannelId = channelId,
                    // ProgramId
                    Name = entry.Name,
                    ServiceName = Name,
                    Overview = entry.Comment,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddHours(1),
                    RecordAnyTime = true,
                    RecordAnyChannel = string.IsNullOrEmpty(channelId),
                    KeepUpTo = Math.Clamp(entry.MaxCount ?? 0, 0, 50),
                    // KeepUntil
                    SkipEpisodesInLibrary = (entry.Record ?? 0) != 0,
                    RecordNewOnly = (entry.Btype ?? 0) == 1 || (entry.Btype ?? 0) == 2,
                    Days = entry.Weekdays
                        .Select(n => n ?? 0)
                        .Where(i => i is >= 1 and <= 7)
                        .Select(i => (DayOfWeek)(i % 7))
                        .Distinct()
                        .ToList(),
                    Priority = entry.Priority ?? 0,
                    PrePaddingSeconds = (entry.StartExtra ?? 0) * 60,
                    PostPaddingSeconds = (entry.StopExtra ?? 0) * 60
                    // IsPrePaddingRequired
                    // IsPostPaddingRequired
                    // SeriesId
                });
            }

            logger.LogInformation("GetSeriesTimers: Retrieved {Count} series timers", timers.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetSeriesTimers: Unexpected error retrieving series timers");
        }

        return timers;
    }

    public async Task<IEnumerable<ProgramInfo>> GetProgramsAsync(string channelId, DateTime startDateUtc,
        DateTime endDateUtc, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(channelId))
        {
            logger.LogWarning("GetPrograms: ChannelId is null or empty");
            throw new ArgumentException("Channel ID cannot be null or empty", nameof(channelId));
        }

        var programs = new List<ProgramInfo>();

        logger.LogDebug("GetPrograms: Requesting programs for channel {ChannelId}", channelId);

        try
        {
            var response = await tvHeadendApiClient
                .GetEpgEventGridAsync(Plugin.ConnectionInfo, channelId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            foreach (var entry in response.Entries)
            {
                if (entry.StopDateTime <= startDateUtc || entry.StartDateTime >= endDateUtc)
                {
                    continue;
                }

                var imageInfo = ImageUtilities.GetImageInfo(entry.Image, appHost);
                var program = new ProgramInfo
                {
                    Id = entry.EventId.ToString(),
                    ChannelId = entry.ChannelUuid,
                    Name = entry.Title,
                    OfficialRating = entry.RatingLabel,
                    Overview = entry.Description,
                    ShortOverview = entry.Summary,
                    StartDate = entry.StartDateTime ?? DateTime.UtcNow,
                    EndDate = entry.StopDateTime ?? DateTime.UtcNow.AddHours(1),
                    // Genres
                    // OriginalAirDate
                    IsHD = entry.Hd,
                    // Is3D
                    // Audio
                    // CommunityRating
                    IsRepeat = entry.Repeat ?? true,
                    // IsSubjectToBlackout
                    EpisodeTitle = entry.Subtitle,
                    // ImagePath
                    ImageUrl = imageInfo.ImageUrl,
                    // ThumbImageUrl
                    // LogoImageUrl
                    // BackdropImageUrl
                    HasImage = imageInfo.HasImage,
                    // IsMovie
                    // IsSports
                    IsSeries = Plugin.Instance.Configuration.ForceAllProgramsAsSeries || !string.IsNullOrEmpty(entry.Subtitle),
                    // IsLive
                    // IsNews
                    // IsKids
                    // IsEducational
                    // IsPremiere
                    ProductionYear = entry.CopyrightYear,
                    // HomePageUrl
                    // SeriesId
                    // ShowId
                    SeasonNumber = entry.SeasonNumber,
                    EpisodeNumber = entry.EpisodeNumber
                    // Etag
                    // ProviderIds
                    // SeriesProviderIds
                };

                programs.Add(program);
            }

            logger.LogInformation("GetPrograms: Retrieved {Count} programs for channel {ChannelId}", programs.Count,
                channelId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "GetPrograms: Unexpected error while requesting programs for channel {ChannelId}", channelId);
        }

        return programs;
    }

    public async Task<MediaSourceInfo> GetChannelStream(string channelId, string streamId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(channelId))
        {
            logger.LogWarning("GetChannelStream: Channel ID is null or empty");
            throw new ArgumentException("Channel ID cannot be null or empty", nameof(channelId));
        }

        if (string.IsNullOrEmpty(streamId))
        {
            var mediaSources = await GetChannelStreamMediaSources(channelId, cancellationToken).ConfigureAwait(false);
            return mediaSources.First();
        }

        var mediaStreams = new List<MediaStream>();

        logger.LogDebug("GetChannelStream: Requesting stream info (streamId: {StreamId})", streamId);

        try
        {
            var streamsResponse = await tvHeadendApiClient
                .GetServiceStreamsAsync(Plugin.ConnectionInfo, streamId, cancellationToken)
                .ConfigureAwait(false);

            foreach (var stream in streamsResponse.Fstreams)
            {
                var codecType = stream.Type?.ToLowerInvariant();
                if (string.IsNullOrEmpty(codecType))
                {
                    continue;
                }

                var mediaStream = new MediaStream { Index = stream.Index - 1 ?? -1, Codec = codecType, Language = stream.Language };

                if (CodecTypes.VideoCodecs.Contains(codecType))
                {
                    mediaStream.Type = MediaStreamType.Video;
                    mediaStream.IsInterlaced = true;
                    mediaStream.Width = stream.Width;
                    mediaStream.Height = stream.Height;

                    if (stream is { AspectNum: > 0, AspectDen: > 0 })
                    {
                        mediaStream.AspectRatio = $"{stream.AspectNum}:{stream.AspectDen}";
                    }
                }
                else if (CodecTypes.AudioCodecs.Contains(codecType))
                {
                    mediaStream.Type = MediaStreamType.Audio;
                }
                else if (CodecTypes.SubtitleCodecs.Contains(codecType))
                {
                    mediaStream.Type = MediaStreamType.Subtitle;
                }
                else
                {
                    continue;
                }

                mediaStreams.Add(mediaStream);
            }

            logger.LogDebug("GetChannelStream: Parsed {Count} media streams (streamId: {StreamId})",
                mediaStreams.Count, streamId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetChannelStream: Error retrieving stream info (streamId: {StreamId})", streamId);
        }

        if (mediaStreams.All(m => m.Type != MediaStreamType.Video))
        {
            logger.LogWarning("GetChannelStream: No video stream found. Adding fallback. (streamId: {StreamId})",
                streamId);
            mediaStreams.Add(new MediaStream { Type = MediaStreamType.Video, Index = -1, IsInterlaced = true });
        }

        if (mediaStreams.All(m => m.Type != MediaStreamType.Audio))
        {
            logger.LogWarning("GetChannelStream: No audio stream found. Adding fallback. (streamId: {StreamId})",
                streamId);
            mediaStreams.Add(new MediaStream { Type = MediaStreamType.Audio, Index = -1 });
        }

        logger.LogDebug("GetChannelStream: Requesting stream ticket (streamId: {StreamId})", streamId);

        try
        {
            var streamUrl = await tvHeadendApiClient.GetPlayUrlForStreamService(Plugin.ConnectionInfo,
                streamId, cancellationToken);

            logger.LogInformation("GetChannelStream: Stream path retrieved successfully (streamId: {StreamId})",
                streamId);

            return new MediaSourceInfo
            {
                Id = streamId,
                Path = streamUrl,
                Protocol = MediaProtocol.Http,
                IsRemote = true,
                AnalyzeDurationMs = 500,
                IsInfiniteStream = true,
                BufferMs = 500,
                FallbackMaxStreamingBitrate = 30000000,
                UseMostCompatibleTranscodingProfile = true,
                MediaStreams = mediaStreams
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetChannelStream: Error retrieving stream ticket (streamId: {StreamId})", streamId);
            throw;
        }
    }

    public async Task<List<MediaSourceInfo>> GetChannelStreamMediaSources(string channelId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(channelId))
        {
            logger.LogWarning("GetChannelStreamMediaSources: Channel ID is null or empty");
            throw new ArgumentException("Channel ID cannot be null or empty", nameof(channelId));
        }

        var mediaSources = new List<MediaSourceInfo>();

        logger.LogDebug("GetChannelStreamMediaSources: Requesting channel grid");

        try
        {
            var response = await tvHeadendApiClient
                .GetChannelGridAsync(Plugin.ConnectionInfo, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var channel = response.Entries.FirstOrDefault(e => e.Uuid == channelId);
            if (channel != null)
            {
                var services = channel.Services;

                logger.LogDebug(
                    "GetChannelStreamMediaSources: Found channel '{ChannelId}' with {ServiceCount} service(s)",
                    channelId, services.Count);

                foreach (var service in services)
                {
                    if (string.IsNullOrEmpty(service))
                    {
                        continue;
                    }

                    var source = await GetChannelStream(channelId, service, cancellationToken).ConfigureAwait(false);
                    mediaSources.Add(source);
                }

                logger.LogInformation(
                    "GetChannelStreamMediaSources: Retrieved {MediaSourceCount} media source(s) for channel '{ChannelId}'",
                    mediaSources.Count, channelId);
            }
            else
            {
                logger.LogWarning("GetChannelStreamMediaSources: Channel '{ChannelId}' not found in response",
                    channelId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "GetChannelStreamMediaSources: Unexpected error retrieving channel '{ChannelId}'", channelId);
        }

        return mediaSources;
    }

    public Task CloseLiveStream(string id, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task ResetTuner(string id, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public string Name => PluginInfo.Name;
    public string HomePageUrl => PluginInfo.HomePageUrl;

    public async Task<string> CreateTimer(TimerInfo info, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(info.ChannelId))
        {
            logger.LogWarning("CreateTimer: ChannelId is null or empty.");
            throw new ArgumentException("Channel ID cannot be null or empty.", nameof(info));
        }

        if (string.IsNullOrEmpty(info.ProgramId))
        {
            if (info.StartDate == default || info.EndDate == default || info.StartDate >= info.EndDate)
            {
                logger.LogWarning("CreateTimer: Invalid start or end date. Start: {StartDate}, End: {EndDate}",
                    info.StartDate, info.EndDate);
                throw new ArgumentException("Invalid start or end date for the timer.", nameof(info));
            }

            logger.LogDebug("CreateTimer: Creating timer for channelId {ChannelId}", info.ChannelId);

            try
            {
                var response = await tvHeadendApiClient.CreateDvrEntryAsync(Plugin.ConnectionInfo,
                    new DvrEntryCreateRequest
                    {
                        ChannelId = info.ChannelId,
                        Title = info.Name,
                        ExtraText = info.Overview,
                        Start = new DateTimeOffset(info.StartDate).ToUnixTimeSeconds(),
                        Stop = new DateTimeOffset(info.EndDate).ToUnixTimeSeconds(),
                        StartExtra = (int)Math.Round(info.PrePaddingSeconds / 60.0),
                        StopExtra = (int)Math.Round(info.PostPaddingSeconds / 60.0)
                    }, cancellationToken);
                return response.Uuid ?? string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateTimer: Failed to create timer for channelId {ChannelId}", info.ChannelId);
                throw;
            }
        }

        logger.LogDebug("CreateTimer: Creating timer for channelId {ChannelId} and programId {ProgramId}",
            info.ChannelId, info.ProgramId);
        try
        {
            var response =
                await tvHeadendApiClient.CreateDvrEntryByEventAsync(Plugin.ConnectionInfo,
                    info.ProgramId, cancellationToken);
            return response.Uuid.First() ?? string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "CreateTimer: Failed to create timer for channelId {ChannelId} and programId {ProgramId}",
                info.ChannelId, info.ProgramId);
            throw;
        }
    }

    public async Task<string> CreateSeriesTimer(SeriesTimerInfo info, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(info.ChannelId))
        {
            logger.LogWarning("CreateSeriesTimer: ChannelId is null or empty.");
            throw new ArgumentException("Channel ID cannot be null or empty.", nameof(info));
        }

        if (string.IsNullOrEmpty(info.ProgramId))
        {
            logger.LogDebug("CreateSeriesTimer: Creating series timer for channelId {ChannelId}", info.ChannelId);

            try
            {
                var request = new DvrAutorecCreateRequest
                {
                    ChannelId = info.RecordAnyChannel ? null : info.ChannelId,
                    Name = info.Name,
                    Title = info.Name,
                    BType = info.RecordNewOnly ? 1 : 0,
                    MaxCount = info.KeepUpTo,
                    Record = info.SkipEpisodesInLibrary ? 14 : 0,
                    StartExtra = (int)Math.Round(info.PrePaddingSeconds / 60.0),
                    StopExtra = (int)Math.Round(info.PostPaddingSeconds / 60.0)
                };
                request.Weekdays.AddRange([1, 2, 3, 4, 5, 6, 7]);
                var response = await tvHeadendApiClient.CreateDvrAutorecAsync(
                    Plugin.ConnectionInfo,
                    request, cancellationToken);
                return response.Uuid ?? string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateSeriesTimer: Failed to create series timer for channelId: {ChannelId}",
                    info.ChannelId);
                throw;
            }
        }

        logger.LogDebug("CreateSeriesTimer: Creating series timer for channelId {ChannelId} and programId {ProgramId}",
            info.ChannelId, info.SeriesId);
        try
        {
            var response =
                await tvHeadendApiClient.CreateDvrAutorecBySeriesAsync(Plugin.ConnectionInfo,
                    info.ProgramId, cancellationToken);
            return response.Uuid.First() ?? string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "CreateSeriesTimer: Failed to create series timer for channelId {ChannelId} and programId {ProgramId}",
                info.ChannelId, info.ProgramId);
            throw;
        }
    }
}
