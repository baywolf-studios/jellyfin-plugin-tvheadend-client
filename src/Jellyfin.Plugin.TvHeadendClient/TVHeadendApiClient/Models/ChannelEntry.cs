using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record ChannelEntry
{
    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }

    [JsonPropertyName("name")] public string? Name { get; init; }

    [JsonPropertyName("number")] public string? Number { get; init; }

    [JsonPropertyName("services")] public List<string?> Services { get; init; } = [];

    [JsonPropertyName("uuid")] public string? Uuid { get; init; }

    [JsonPropertyName("icon_public_url")] public string? IconPublicUrl { get; init; }
}
