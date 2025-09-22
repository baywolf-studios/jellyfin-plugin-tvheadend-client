using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrEntryCreateByEventResponse
{
    [JsonPropertyName("uuid")] public List<string?> Uuid { get; init; } = [];
}