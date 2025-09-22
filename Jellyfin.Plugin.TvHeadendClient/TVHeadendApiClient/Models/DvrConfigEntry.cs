using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrConfigEntry
{
    [JsonPropertyName("cache")] public int? Cache { get; init; }

    [JsonPropertyName("storage-mfree")] public int? StorageMfree { get; init; }

    [JsonPropertyName("autorec-maxsched")] public int? AutorecMaxsched { get; init; }

    [JsonPropertyName("whitespace-in-title")]
    public bool? WhitespaceInTitle { get; init; }

    [JsonPropertyName("time-in-title")] public bool? TimeInTitle { get; init; }

    [JsonPropertyName("channel-dir")] public bool? ChannelDir { get; init; }

    [JsonPropertyName("epg-running")] public bool? EpgRunning { get; init; }

    [JsonPropertyName("name")] public string? Name { get; init; }

    [JsonPropertyName("episode-in-title")] public bool? EpisodeInTitle { get; init; }

    [JsonPropertyName("profile")] public string? Profile { get; init; }

    [JsonPropertyName("clean-title")] public bool? CleanTitle { get; init; }

    [JsonPropertyName("pre-extra-time")] public int? PreExtraTime { get; init; }

    [JsonPropertyName("uuid")] public string? Uuid { get; init; }

    [JsonPropertyName("day-dir")] public bool? DayDir { get; init; }

    [JsonPropertyName("subtitle-in-title")]
    public bool? SubtitleInTitle { get; init; }

    [JsonPropertyName("epg-update-window")]
    public int? EpgUpdateWindow { get; init; }

    [JsonPropertyName("title-dir")] public bool? TitleDir { get; init; }

    [JsonPropertyName("warm-time")] public int? WarmTime { get; init; }

    [JsonPropertyName("windows-compatible-filenames")]
    public bool? WindowsCompatibleFilenames { get; init; }

    [JsonPropertyName("autorec-maxcount")] public int? AutorecMaxcount { get; init; }

    [JsonPropertyName("storage-mused")] public int? StorageMused { get; init; }

    [JsonPropertyName("tag-files")] public bool? TagFiles { get; init; }

    [JsonPropertyName("omit-title")] public bool? OmitTitle { get; init; }

    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }

    [JsonPropertyName("storage")] public string? Storage { get; init; }

    [JsonPropertyName("date-in-title")] public bool? DateInTitle { get; init; }

    [JsonPropertyName("channel-in-title")] public bool? ChannelInTitle { get; init; }

    [JsonPropertyName("post-extra-time")] public int? PostExtraTime { get; init; }

    [JsonPropertyName("skip-commercials")] public bool? SkipCommercials { get; init; }

    [JsonPropertyName("rerecord-errors")] public int? RerecordErrors { get; init; }

    [JsonPropertyName("file-permissions")] public string? FilePermissions { get; init; }

    [JsonPropertyName("retention-days")] public int? RetentionDays { get; init; }

    [JsonPropertyName("charset")] public string? Charset { get; init; }

    [JsonPropertyName("pri")] public int? Priority { get; init; }

    [JsonPropertyName("directory-permissions")]
    public string? DirectoryPermissions { get; init; }

    [JsonPropertyName("pathname")] public string? Pathname { get; init; }

    [JsonPropertyName("remove-after-playback")]
    public int? RemoveAfterPlayback { get; init; }

    [JsonPropertyName("complex-scheduling")]
    public bool? ComplexScheduling { get; init; }

    [JsonPropertyName("fetch-artwork")] public bool? FetchArtwork { get; init; }

    [JsonPropertyName("fetch-artwork-known-broadcasts-allow-unknown")]
    public bool? FetchArtworkKnownBroadcastsAllowUnknown { get; init; }

    [JsonPropertyName("format-tvmovies-subdir")]
    public string? FormatTvmoviesSubdir { get; init; }

    [JsonPropertyName("format-tvshows-subdir")]
    public string? FormatTvshowsSubdir { get; init; }

    [JsonPropertyName("create-scene-markers")]
    public bool? CreateSceneMarkers { get; init; }

    [JsonPropertyName("record")] public int? Record { get; init; }
}