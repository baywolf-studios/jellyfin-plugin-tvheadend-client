using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record ServiceStreamsResponse
{
    [JsonPropertyName("name")] public string? Name { get; init; }

    [JsonPropertyName("streams")] public List<Stream> Streams { get; init; } = [];

    [JsonPropertyName("fstreams")] public List<Fstream> Fstreams { get; init; } = [];
}