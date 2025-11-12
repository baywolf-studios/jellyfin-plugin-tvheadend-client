using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrAutorecCreateRequest
{
    [JsonPropertyName("name")] public string? Name { get; init; }

    [JsonPropertyName("title")] public string? Title { get; init; }

    [JsonPropertyName("channel")] public string? ChannelId { get; init; }

    [JsonPropertyName("weekdays")] public List<int?> Weekdays { get; init; } = [];

    [JsonPropertyName("record")] public int? Record { get; init; }

    [JsonPropertyName("btype")] public int? BType { get; init; }
    [JsonPropertyName("maxcount")] public int? MaxCount { get; init; }

    [JsonPropertyName("start_extra")] public int? StartExtra { get; init; }

    [JsonPropertyName("stop_extra")] public int? StopExtra { get; init; }
}
