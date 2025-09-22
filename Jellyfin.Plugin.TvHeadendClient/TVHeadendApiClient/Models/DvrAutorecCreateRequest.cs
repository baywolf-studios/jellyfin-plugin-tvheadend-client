using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrAutorecCreateRequest
{
    [JsonPropertyName("name")] public string? Name { get; init; }

    [JsonPropertyName("title")] public string? Title { get; init; }

    [JsonPropertyName("fulltext")] public bool? FullText { get; init; }

    [JsonPropertyName("channel")] public string? ChannelId { get; init; }

    [JsonPropertyName("start")] public string? Start { get; init; }

    [JsonPropertyName("start_window")] public string? StartWindow { get; init; }

    [JsonPropertyName("weekdays")] public List<int?> Weekdays { get; init; } = [];

    [JsonPropertyName("comment")] public string? Comment { get; init; }

    [JsonPropertyName("record")] public int? Record { get; init; }

    [JsonPropertyName("tag")] public string? Tag { get; init; }

    [JsonPropertyName("btype")] public int? BType { get; init; }

    [JsonPropertyName("content_type")] public int? ContentType { get; init; }

    [JsonPropertyName("config_name")] public string? ConfigName { get; init; }

    [JsonPropertyName("pri")] public int? Priority { get; init; }

    [JsonPropertyName("cat1")] public string? Cat1 { get; init; }

    [JsonPropertyName("cat2")] public string? Cat2 { get; init; }

    [JsonPropertyName("cat3")] public string? Cat3 { get; init; }

    [JsonPropertyName("minduration")] public int? MinDuration { get; init; }

    [JsonPropertyName("maxduration")] public int? MaxDuration { get; init; }

    [JsonPropertyName("minyear")] public int? MinYear { get; init; }

    [JsonPropertyName("maxyear")] public int? MaxYear { get; init; }

    [JsonPropertyName("minseason")] public int? MinSeason { get; init; }

    [JsonPropertyName("maxseason")] public int? MaxSeason { get; init; }

    [JsonPropertyName("star_rating")] public int? StarRating { get; init; }

    [JsonPropertyName("directory")] public string? Directory { get; init; }

    [JsonPropertyName("maxcount")] public int? MaxCount { get; init; }

    [JsonPropertyName("start_extra")] public int? StartExtra { get; init; }

    [JsonPropertyName("stop_extra")] public int? StopExtra { get; init; }
}