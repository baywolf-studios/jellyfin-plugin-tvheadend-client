using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrConfigGridResponse
{
    [JsonPropertyName("entries")] public List<DvrConfigEntry> Entries { get; init; } = [];

    [JsonPropertyName("total")] public int? Total { get; init; }
}
