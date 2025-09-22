using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record ChannelGridResponse
{
    [JsonPropertyName("entries")] public List<ChannelEntry> Entries { get; init; } = [];

    [JsonPropertyName("total")] public int? Total { get; init; }
}