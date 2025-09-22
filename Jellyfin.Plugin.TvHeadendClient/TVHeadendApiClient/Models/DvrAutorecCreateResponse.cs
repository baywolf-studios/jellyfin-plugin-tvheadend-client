using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrAutorecCreateResponse
{
    [JsonPropertyName("uuid")] public string? Uuid { get; init; }
}