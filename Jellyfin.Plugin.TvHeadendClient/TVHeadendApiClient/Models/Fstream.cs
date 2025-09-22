using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record Fstream
{
    [JsonPropertyName("index")] public int? Index { get; init; }

    [JsonPropertyName("pid")] public int? Pid { get; init; }

    [JsonPropertyName("type")] public string? Type { get; init; }

    [JsonPropertyName("language")] public string? Language { get; init; }

    [JsonPropertyName("width")] public int? Width { get; init; }

    [JsonPropertyName("height")] public int? Height { get; init; }

    [JsonPropertyName("duration")] public int? Duration { get; init; }

    [JsonPropertyName("aspect_num")] public int? AspectNum { get; init; }

    [JsonPropertyName("aspect_den")] public int? AspectDen { get; init; }

    [JsonPropertyName("audio_type")] public int? AudioType { get; init; }

    [JsonPropertyName("composition_id")] public int? CompositionId { get; init; }

    [JsonPropertyName("ancillary_id")] public int? AncillaryId { get; init; }
}