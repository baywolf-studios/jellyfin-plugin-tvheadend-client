using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrEventEntry
{
    [JsonPropertyName("uuid")] public string? Uuid { get; init; }

    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }

    [JsonPropertyName("start_extra")] public int? StartExtra { get; init; }

    [JsonPropertyName("start_real")] public long? StartReal { get; init; }

    [JsonIgnore]
    public DateTime? StartRealDateTime =>
        StartReal.HasValue ? DateTimeOffset.FromUnixTimeSeconds(StartReal.Value).UtcDateTime : null;

    [JsonPropertyName("stop_extra")] public int? StopExtra { get; init; }

    [JsonPropertyName("stop_real")] public long? StopReal { get; init; }

    [JsonIgnore] public DateTime? StopRealDateTime => StopReal.HasValue ? DateTimeOffset.FromUnixTimeSeconds(StopReal.Value).UtcDateTime : null;

    [JsonPropertyName("duration")] public int? Duration { get; init; }

    [JsonPropertyName("channel")] public string? Channel { get; init; }

    [JsonPropertyName("image")] public string? Image { get; init; }

    [JsonPropertyName("disp_title")] public string? Title { get; init; }

    [JsonPropertyName("disp_subtitle")] public string? Subtitle { get; init; }

    [JsonPropertyName("disp_description")] public string? Description { get; init; }

    [JsonPropertyName("pri")] public int? Priority { get; init; }

    [JsonPropertyName("config_name")] public string? ConfigName { get; init; }

    [JsonPropertyName("filename")] public string? Filename { get; init; }

    [JsonPropertyName("autorec")] public string? Autorec { get; init; }

    [JsonPropertyName("parent")] public string? Parent { get; init; }

    [JsonPropertyName("copyright_year")] public int? CopyrightYear { get; init; }

    [JsonPropertyName("broadcast")] public int? Broadcast { get; init; }

    [JsonPropertyName("url")] public string? Url { get; init; }

    [JsonPropertyName("sched_status")] public string? SchedStatus { get; init; }

    [JsonPropertyName("duplicate")] public bool? Duplicate { get; init; }

    [JsonPropertyName("first_aired")] public int? FirstAired { get; init; }

    [JsonIgnore]
    public DateTime? FirstAiredDateTime =>
        FirstAired is > 0
            ? DateTimeOffset.FromUnixTimeSeconds(FirstAired.Value).UtcDateTime
            : null;

    [JsonPropertyName("rating_label")] public string? RatingLabel { get; init; }
}
