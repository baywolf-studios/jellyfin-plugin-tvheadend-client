using System.Reflection;

namespace Jellyfin.Plugin.TvHeadendClient;

public static class PluginInfo
{
    public const string Name = "TvHeadend Client";
    public const string HomePageUrl = "https://tvheadend.org";
    public const string IdString = "d2762daa-c6a2-4ede-b513-36b09a9f1752";
    public static readonly Guid Id = Guid.Parse(IdString);
    public static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
}
