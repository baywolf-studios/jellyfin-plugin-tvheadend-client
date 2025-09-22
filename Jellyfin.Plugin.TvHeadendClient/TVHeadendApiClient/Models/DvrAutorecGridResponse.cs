using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrAutorecGridResponse
{
    [JsonPropertyName("entries")] public List<DvrAutorecEntry> Entries { get; init; } = [];

    [JsonPropertyName("total")] public int? Total { get; init; }
}