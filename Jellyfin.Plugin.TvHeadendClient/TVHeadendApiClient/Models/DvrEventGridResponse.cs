using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrEventGridResponse
{
    [JsonPropertyName("entries")] public List<DvrEventEntry> Entries { get; init; } = [];

    [JsonPropertyName("total")] public int? Total { get; init; }
}