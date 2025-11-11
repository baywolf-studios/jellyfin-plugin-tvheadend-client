using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record EpgEventEntry
{
    [JsonPropertyName("eventId")] public int? EventId { get; init; }

    [JsonPropertyName("channelUuid")] public string? ChannelUuid { get; init; }

    [JsonPropertyName("start")] public long? Start { get; init; }

    [JsonIgnore]
    public DateTime? StartDateTime =>
        Start.HasValue ? DateTimeOffset.FromUnixTimeSeconds(Start.Value).UtcDateTime : null;

    [JsonPropertyName("stop")] public long? Stop { get; init; }

    [JsonIgnore] public DateTime? StopDateTime => Stop.HasValue ? DateTimeOffset.FromUnixTimeSeconds(Stop.Value).UtcDateTime : null;

    [JsonPropertyName("title")] public string? Title { get; init; }

    [JsonPropertyName("summary")] public string? Summary { get; init; }

    [JsonPropertyName("hd")] public bool? Hd { get; init; }

    [JsonPropertyName("ratingLabel")] public string? RatingLabel { get; init; }

    [JsonPropertyName("description")] public string? Description { get; init; }

    [JsonPropertyName("subtitle")] public string? Subtitle { get; init; }

    [JsonPropertyName("repeat")] public bool? Repeat { get; init; }

    [JsonPropertyName("seasonNumber")] public int? SeasonNumber { get; init; }

    [JsonPropertyName("episodeNumber")] public int? EpisodeNumber { get; init; }

    [JsonPropertyName("image")] public string? Image { get; init; }

    [JsonPropertyName("copyright_year")] public int? CopyrightYear { get; init; }
}
