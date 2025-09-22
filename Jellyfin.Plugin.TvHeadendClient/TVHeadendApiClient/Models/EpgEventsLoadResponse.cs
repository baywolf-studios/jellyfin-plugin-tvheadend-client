using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record EpgEventsLoadResponse
{
    [JsonPropertyName("entries")] public List<EpgEventsLoadEntry> Entries { get; init; } = [];

    [JsonPropertyName("totalCount")] public int? TotalCount { get; set; }
}