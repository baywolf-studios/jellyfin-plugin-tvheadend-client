using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrConfigEntry
{
    [JsonPropertyName("name")] public string? Name { get; init; }

    [JsonPropertyName("pre-extra-time")] public int? PreExtraTime { get; init; }

    [JsonPropertyName("uuid")] public string? Uuid { get; init; }

    [JsonPropertyName("autorec-maxcount")] public int? AutorecMaxcount { get; init; }

    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }

    [JsonPropertyName("storage")] public string? Storage { get; init; }

    [JsonPropertyName("post-extra-time")] public int? PostExtraTime { get; init; }

    [JsonPropertyName("pri")] public int? Priority { get; init; }

    [JsonPropertyName("record")] public int? Record { get; init; }
}
