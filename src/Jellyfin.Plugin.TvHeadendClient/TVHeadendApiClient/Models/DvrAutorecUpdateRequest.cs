using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrAutorecUpdateRequest
{
    [JsonPropertyName("name")] public string? Name { get; set; }

    [JsonPropertyName("title")] public string? Title { get; set; }

    [JsonPropertyName("channel")] public string? ChannelId { get; set; }

    [JsonPropertyName("weekdays")] public List<int?> Weekdays { get; init; } = [];

    [JsonPropertyName("start_extra")] public int? StartExtra { get; set; }

    [JsonPropertyName("stop_extra")] public int? StopExtra { get; set; }

    [JsonPropertyName("record")] public int? Record { get; set; }

    [JsonPropertyName("btype")] public int? BType { get; set; }

    [JsonPropertyName("maxcount")] public int? MaxCount { get; set; }

    [JsonPropertyName("uuid")] public string? Uuid { get; set; }
}
