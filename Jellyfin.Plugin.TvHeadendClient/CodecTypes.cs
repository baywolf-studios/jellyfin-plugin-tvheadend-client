namespace Jellyfin.Plugin.TvHeadendClient;

public static class CodecTypes
{
    public static readonly HashSet<string> VideoCodecs = new()
    {
        "h264",
        "mpeg2video",
        "hevc",
        "h265",
        "vp8",
        "vp9"
    };

    public static readonly HashSet<string> AudioCodecs = new()
    {
        "aac",
        "ac3",
        "eac3",
        "mp2",
        "mp3",
        "opus",
        "vorbis"
    };

    public static readonly HashSet<string> SubtitleCodecs = new() { "dvbsub", "teletext", "subrip" };
}