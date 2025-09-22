using Jellyfin.Plugin.TvHeadendClient.Configuration;
using Jellyfin.Plugin.TvHeadendClient.Http;

namespace Jellyfin.Plugin.TvHeadendClient.Helpers;

public static class PluginConfigurationExtensions
{
    public static DigestConnectionInfo ToConnectionInfo(this PluginConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config);

        return new DigestConnectionInfo
        {
            Host = config.Host,
            Port = config.Port,
            UseSsl = config.UseSsl,
            Username = string.IsNullOrWhiteSpace(config.Username) ? null : config.Username,
            Password = string.IsNullOrWhiteSpace(config.Password) ? null : config.Password
        };
    }
}