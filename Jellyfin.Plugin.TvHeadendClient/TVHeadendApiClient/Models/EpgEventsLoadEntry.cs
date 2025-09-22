using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record EpgEventsLoadEntry
{
    [JsonPropertyName("eventId")] public string? EventId { get; init; }

    [JsonPropertyName("episodeUri")] public string? EpisodeUri { get; init; }

    [JsonPropertyName("serieslinkUri")] public string? SeriesLinkUri { get; init; }

    [JsonPropertyName("channelName")] public string? ChannelName { get; init; }

    [JsonPropertyName("channelUuid")] public string? ChannelUuid { get; init; }

    [JsonPropertyName("channelNumber")] public string? ChannelNumber { get; init; }

    [JsonPropertyName("start")] public int? Start { get; init; }

    [JsonPropertyName("stop")] public int? Stop { get; init; }

    [JsonPropertyName("title")] public string? Title { get; init; }

    [JsonPropertyName("summary")] public string? Summary { get; init; }

    [JsonPropertyName("widescreen")] public int? Widescreen { get; init; }

    [JsonPropertyName("subtitled")] public int? Subtitled { get; init; }

    [JsonPropertyName("hd")] public int? Hd { get; init; }

    [JsonPropertyName("genre")] public List<int?> Genre { get; init; } = [];

    [JsonPropertyName("nextEventId")] public int? NextEventId { get; init; }

    [JsonPropertyName("ageRating")] public int? AgeRating { get; init; }

    [JsonPropertyName("ratingLabel")] public string? RatingLabel { get; init; }

    [JsonPropertyName("ratingLabelIcon")] public string? RatingLabelIcon { get; init; }

    [JsonPropertyName("channelIcon")] public string? ChannelIcon { get; init; }

    [JsonPropertyName("subtitle")] public string? Subtitle { get; init; }

    [JsonPropertyName("description")] public string? Description { get; init; }

    [JsonPropertyName("category")] public List<string?> Category { get; init; } = [];

    [JsonPropertyName("keyword")] public List<string?> Keyword { get; init; } = [];

    [JsonPropertyName("repeat")] public int? Repeat { get; init; }

    [JsonPropertyName("seasonNumber")] public int? SeasonNumber { get; init; }

    [JsonPropertyName("episodeNumber")] public int? EpisodeNumber { get; init; }

    [JsonPropertyName("episodeOnscreen")] public string? EpisodeOnscreen { get; init; }

    [JsonPropertyName("image")] public string? Image { get; init; }

    [JsonPropertyName("copyright_year")] public int? CopyrightYear { get; init; }
}