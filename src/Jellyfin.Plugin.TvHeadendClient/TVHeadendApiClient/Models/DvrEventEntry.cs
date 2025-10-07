using System.Text.Json.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

public record DvrEventEntry
{
    [JsonPropertyName("uuid")] public string? Uuid { get; init; }

    [JsonPropertyName("enabled")] public bool? Enabled { get; init; }

    [JsonPropertyName("create")] public int? Create { get; init; }

    [JsonPropertyName("watched")] public int? Watched { get; init; }

    [JsonPropertyName("start")] public long? Start { get; init; }

    [JsonIgnore]
    public DateTime? StartDateTime =>
        Start.HasValue ? DateTimeOffset.FromUnixTimeSeconds(Start.Value).UtcDateTime : null;

    [JsonPropertyName("start_extra")] public int? StartExtra { get; init; }

    [JsonPropertyName("start_real")] public long? StartReal { get; init; }

    [JsonPropertyName("stop")] public long? Stop { get; init; }

    [JsonIgnore] public DateTime? StopDateTime => Stop.HasValue ? DateTimeOffset.FromUnixTimeSeconds(Stop.Value).UtcDateTime : null;

    [JsonPropertyName("stop_extra")] public int? StopExtra { get; init; }

    [JsonPropertyName("stop_real")] public long? StopReal { get; init; }

    [JsonPropertyName("duration")] public int? Duration { get; init; }

    [JsonPropertyName("channel")] public string? Channel { get; init; }

    [JsonPropertyName("channelname")] public string? ChannelName { get; init; }

    [JsonPropertyName("image")] public string? Image { get; init; }

    [JsonPropertyName("fanart_image")] public string? FanartImage { get; init; }

    [JsonPropertyName("disp_title")] public string? Title { get; init; }

    [JsonPropertyName("disp_subtitle")] public string? Subtitle { get; init; }

    [JsonPropertyName("disp_summary")] public string? Summary { get; init; }

    [JsonPropertyName("disp_description")] public string? Description { get; init; }

    [JsonPropertyName("disp_extratext")] public string? ExtraText { get; init; }

    [JsonPropertyName("pri")] public int? Priority { get; init; }

    [JsonPropertyName("retention")] public int? Retention { get; init; }

    [JsonPropertyName("removal")] public long? Removal { get; init; }

    [JsonPropertyName("playposition")] public int? PlayPosition { get; init; }

    [JsonPropertyName("playcount")] public int? Playcount { get; init; }

    [JsonPropertyName("config_name")] public string? ConfigName { get; init; }

    [JsonPropertyName("owner")] public string? Owner { get; init; }

    [JsonPropertyName("creator")] public string? Creator { get; init; }

    [JsonPropertyName("filename")] public string? Filename { get; init; }

    [JsonPropertyName("errorcode")] public int? ErrorCode { get; init; }

    [JsonPropertyName("errors")] public int? Errors { get; init; }

    [JsonPropertyName("data_errors")] public int? DataErrors { get; init; }

    [JsonPropertyName("dvb_eid")] public int? DvbEid { get; init; }

    [JsonPropertyName("noresched")] public bool? NoResched { get; init; }

    [JsonPropertyName("norerecord")] public bool? NoRerecord { get; init; }

    [JsonPropertyName("fileremoved")] public int? FileRemoved { get; init; }

    [JsonPropertyName("uri")] public string? Uri { get; init; }

    [JsonPropertyName("autorec")] public string? Autorec { get; init; }

    [JsonPropertyName("autorec_caption")] public string? AutorecCaption { get; init; }

    [JsonPropertyName("timerec")] public string? TimeRec { get; init; }

    [JsonPropertyName("timerec_caption")] public string? TimeRecCaption { get; init; }

    [JsonPropertyName("parent")] public string? Parent { get; init; }

    [JsonPropertyName("child")] public string? Child { get; init; }

    [JsonPropertyName("content_type")] public int? ContentType { get; init; }

    [JsonPropertyName("copyright_year")] public int? CopyrightYear { get; init; }

    [JsonPropertyName("broadcast")] public int? Broadcast { get; init; }

    [JsonPropertyName("url")] public string? Url { get; init; }

    [JsonPropertyName("filesize")] public long? Filesize { get; init; }

    [JsonPropertyName("status")] public string? Status { get; init; }

    [JsonPropertyName("sched_status")] public string? SchedStatus { get; init; }

    [JsonPropertyName("duplicate")] public bool? Duplicate { get; init; }

    [JsonPropertyName("first_aired")] public int? FirstAired { get; init; }

    [JsonIgnore]
    public DateTime? FirstAiredDateTime =>
        FirstAired is > 0
            ? DateTimeOffset.FromUnixTimeSeconds(FirstAired.Value).UtcDateTime
            : null;

    [JsonPropertyName("comment")] public string? Comment { get; init; }

    [JsonPropertyName("category")] public List<string?> Category { get; init; } = [];

    [JsonPropertyName("keyword")] public List<string?> Keyword { get; init; } = [];

    [JsonPropertyName("genre")] public List<int?> Genre { get; init; } = [];

    [JsonPropertyName("age_rating")] public int? AgeRating { get; init; }

    [JsonPropertyName("rating_label_saved")]
    public string? RatingLabelSaved { get; init; }

    [JsonPropertyName("rating_icon_saved")]
    public string? RatingIconSaved { get; init; }

    [JsonPropertyName("rating_country_saved")]
    public string? RatingCountrySaved { get; init; }

    [JsonPropertyName("rating_authority_saved")]
    public string? RatingAuthoritySaved { get; init; }

    [JsonPropertyName("rating_label_uuid")]
    public string? RatingLabelUuid { get; init; }

    [JsonPropertyName("rating_icon")] public string? RatingIcon { get; init; }

    [JsonPropertyName("rating_label")] public string? RatingLabel { get; init; }

    [JsonPropertyName("channel_icon")] public string? ChannelIcon { get; init; }

    [JsonPropertyName("rating_authority")] public string? RatingAuthority { get; init; }

    [JsonPropertyName("rating_country")] public string? RatingCountry { get; init; }
}
