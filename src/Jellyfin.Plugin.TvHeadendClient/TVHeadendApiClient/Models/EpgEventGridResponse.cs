using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record EpgEventGridResponse
{
    [JsonPropertyName("totalCount")] public int? TotalCount { get; init; }

    [JsonPropertyName("entries")] public List<EpgEventEntry> Entries { get; init; } = [];
}
