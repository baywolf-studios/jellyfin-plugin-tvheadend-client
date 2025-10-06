using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrEntryCreateResponse
{
    [JsonPropertyName("uuid")] public string? Uuid { get; set; }
}
