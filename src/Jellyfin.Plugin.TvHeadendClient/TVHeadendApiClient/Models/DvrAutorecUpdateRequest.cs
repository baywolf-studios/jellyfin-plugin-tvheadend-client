using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrAutorecUpdateRequest
{
    [JsonPropertyName("enabled")] public bool? Enabled { get; set; }

    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("fulltext")] public bool? Fulltext { get; set; }

    [JsonPropertyName("channel")] public string? ChannelId { get; set; }

    [JsonPropertyName("start")] public string? Start { get; set; }

    [JsonPropertyName("start_window")] public string? StartWindow { get; set; }

    [JsonPropertyName("weekdays")] public List<int?> Weekdays { get; init; } = [];

    [JsonPropertyName("comment")] public string? Comment { get; set; }

    [JsonPropertyName("start_extra")] public int? StartExtra { get; set; }

    [JsonPropertyName("stop_extra")] public int? StopExtra { get; set; }

    [JsonPropertyName("record")] public int? Record { get; set; }

    [JsonPropertyName("tag")] public string? Tag { get; set; }

    [JsonPropertyName("btype")] public int? BType { get; set; }

    [JsonPropertyName("content_type")] public int? ContentType { get; set; }

    [JsonPropertyName("config_name")] public string? ConfigName { get; set; }

    [JsonPropertyName("owner")] public string? Owner { get; set; }

    [JsonPropertyName("creator")] public string? Creator { get; set; }

    [JsonPropertyName("pri")] public int? Priority { get; set; }

    [JsonPropertyName("removal")] public long? Removal { get; set; }

    [JsonPropertyName("cat1")] public string? Cat1 { get; set; }

    [JsonPropertyName("cat2")] public string? Cat2 { get; set; }

    [JsonPropertyName("cat3")] public string? Cat3 { get; set; }

    [JsonPropertyName("minduration")] public int? MinDuration { get; set; }

    [JsonPropertyName("maxduration")] public int? MaxDuration { get; set; }

    [JsonPropertyName("minyear")] public int? MinYear { get; set; }

    [JsonPropertyName("maxyear")] public int? MaxYear { get; set; }

    [JsonPropertyName("minseason")] public int? MinSeason { get; set; }

    [JsonPropertyName("maxseason")] public int? MaxSeason { get; set; }

    [JsonPropertyName("star_rating")] public int? StarRating { get; set; }

    [JsonPropertyName("directory")] public string? Directory { get; set; }

    [JsonPropertyName("retention")] public int? Retention { get; set; }

    [JsonPropertyName("maxcount")] public int? MaxCount { get; set; }

    [JsonPropertyName("maxsched")] public int? MaxSched { get; set; }

    [JsonPropertyName("uuid")] public string? Uuid { get; set; }
}
