using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record ServiceStreamsResponse
{
    [JsonPropertyName("fstreams")] public List<Fstream> Fstreams { get; init; } = [];
}
