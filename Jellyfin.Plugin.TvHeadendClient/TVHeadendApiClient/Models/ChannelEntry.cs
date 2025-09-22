using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record ChannelEntry
{
    [JsonPropertyName("dvr_pst_time")] public int? DvrPstTime { get; init; }

    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }

    [JsonPropertyName("epgauto")] public bool? EpgAuto { get; init; }

    [JsonPropertyName("autoname")] public bool? AutoName { get; init; }

    [JsonPropertyName("name")] public string? Name { get; init; }

    [JsonPropertyName("number")] public string? Number { get; init; }

    [JsonPropertyName("tags")] public List<string?> Tags { get; init; } = [];

    [JsonPropertyName("services")] public List<string?> Services { get; init; } = [];

    [JsonPropertyName("epg_running")] public int? EpgRunning { get; init; }

    [JsonPropertyName("uuid")] public string? Uuid { get; init; }

    [JsonPropertyName("bouquet")] public string? Bouquet { get; init; }

    [JsonPropertyName("dvr_pre_time")] public int? DvrPreTime { get; init; }

    [JsonPropertyName("icon")] public string? Icon { get; init; }

    [JsonPropertyName("icon_public_url")] public string? IconPublicUrl { get; init; }

    [JsonPropertyName("epglimit")] public int? EpgLimit { get; init; }

    [JsonPropertyName("remote_timeshift")] public bool? RemoteTimeshift { get; init; }
}