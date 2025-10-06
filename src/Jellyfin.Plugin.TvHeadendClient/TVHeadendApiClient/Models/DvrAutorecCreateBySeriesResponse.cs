using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrAutorecCreateBySeriesResponse
{
    [JsonPropertyName("uuid")] public List<string?> Uuid { get; init; } = [];
}
