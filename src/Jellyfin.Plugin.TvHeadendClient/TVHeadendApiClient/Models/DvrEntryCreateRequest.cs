using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrEntryCreateRequest
{
    [JsonPropertyName("disp_title")] public string? Title { get; init; }

    [JsonPropertyName("disp_extratext")] public string? ExtraText { get; init; }

    [JsonPropertyName("channel")] public string? ChannelId { get; init; }

    [JsonPropertyName("start")] public long? Start { get; init; }

    [JsonPropertyName("stop")] public long? Stop { get; init; }

    [JsonPropertyName("comment")] public string? Comment { get; init; }

    [JsonPropertyName("start_extra")] public int? StartExtra { get; init; }

    [JsonPropertyName("stop_extra")] public int? StopExtra { get; init; }

    [JsonPropertyName("pri")] public int? Priority { get; init; }

    [JsonPropertyName("config_name")] public string? ConfigName { get; init; }
}
