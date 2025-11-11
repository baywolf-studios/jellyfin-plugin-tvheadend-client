using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrAutorecEntry
{
    [JsonPropertyName("record")] public int? Record { get; init; }

    [JsonPropertyName("pri")] public int? Priority { get; init; }

    [JsonPropertyName("channel")] public string? Channel { get; init; }

    [JsonPropertyName("start_extra")] public int? StartExtra { get; init; }

    [JsonPropertyName("stop_extra")] public int? StopExtra { get; init; }

    [JsonPropertyName("maxcount")] public int? MaxCount { get; init; }

    [JsonPropertyName("weekdays")] public List<int?> Weekdays { get; init; } = [];

    [JsonPropertyName("uuid")] public string? Uuid { get; init; }

    [JsonPropertyName("comment")] public string? Comment { get; init; }

    [JsonPropertyName("btype")] public int? Btype { get; init; }

    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }
    [JsonPropertyName("name")] public string? Name { get; init; }
}
