using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrAutorecEntry
{
    [JsonPropertyName("record")] public int? Record { get; init; }

    [JsonPropertyName("pri")] public int? Priority { get; init; }

    [JsonPropertyName("channel")] public string? Channel { get; init; }

    [JsonPropertyName("content_type")] public int? ContentType { get; init; }

    [JsonPropertyName("retention")] public int? Retention { get; init; }

    [JsonPropertyName("fulltext")] public bool? Fulltext { get; init; }

    [JsonPropertyName("start_window")] public string? StartWindow { get; init; }

    [JsonPropertyName("serieslink")] public string? SeriesLink { get; init; }

    [JsonPropertyName("config_name")] public string? ConfigName { get; init; }

    [JsonPropertyName("maxduration")] public int? MaxDuration { get; init; }

    [JsonPropertyName("removal")] public int? Removal { get; init; }

    [JsonPropertyName("start_extra")] public int? StartExtra { get; init; }

    [JsonPropertyName("stop_extra")] public int? StopExtra { get; init; }

    [JsonPropertyName("maxcount")] public int? MaxCount { get; init; }

    [JsonPropertyName("owner")] public string? Owner { get; init; }

    [JsonPropertyName("weekdays")] public List<int?> Weekdays { get; init; } = [];

    [JsonPropertyName("uuid")] public string? Uuid { get; init; }

    [JsonPropertyName("creator")] public string? Creator { get; init; }

    [JsonPropertyName("tag")] public string? Tag { get; init; }

    [JsonPropertyName("maxsched")] public int? MaxSched { get; init; }

    [JsonPropertyName("comment")] public string? Comment { get; init; }

    [JsonPropertyName("start")] public string? Start { get; init; }

    [JsonPropertyName("btype")] public int? Btype { get; init; }

    [JsonPropertyName("brand")] public string? Brand { get; init; }

    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }

    [JsonPropertyName("season")] public string? Season { get; init; }

    [JsonPropertyName("minduration")] public int? MinDuration { get; init; }

    [JsonPropertyName("title")] public string? Title { get; init; }

    [JsonPropertyName("name")] public string? Name { get; init; }

    [JsonPropertyName("star_rating")] public int? StarRating { get; init; }

    [JsonPropertyName("minyear")] public int? MinYear { get; init; }

    [JsonPropertyName("maxyear")] public int? MaxYear { get; init; }

    [JsonPropertyName("minseason")] public int? MinSeason { get; init; }

    [JsonPropertyName("maxseason")] public int? MaxSeason { get; init; }
}