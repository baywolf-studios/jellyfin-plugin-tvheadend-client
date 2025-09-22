using System.Globalization;
using Jellyfin.Plugin.TvHeadendClient.Configuration;
using Jellyfin.Plugin.TvHeadendClient.Helpers;
using Jellyfin.Plugin.TvHeadendClient.Http;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.TvHeadendClient;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private static Plugin? _instance;
    
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        _instance = this;
    }

    public override string Name => PluginInfo.Name;

    public override Guid Id => PluginInfo.Id;

    public static Plugin Instance => _instance ?? throw new InvalidOperationException("Plugin instance not available");
    
    public static DigestConnectionInfo ConnectionInfo => Instance.Configuration.ToConnectionInfo();

    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html",
                    GetType().Namespace)
            }
        ];
    }
}