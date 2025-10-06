using System.Net.Http.Headers;
using Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.TvHeadendClient;

public class ServiceRegistrator : IPluginServiceRegistrator
{
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddHttpClient(PluginInfo.Name, client =>
        {
            var productHeaderValue = new ProductHeaderValue("Jellyfin.Plugin.TvHeadendClient", PluginInfo.Version);
            var productInfoHeaderValue = new ProductInfoHeaderValue(productHeaderValue);
            client.DefaultRequestHeaders.UserAgent.Add(productInfoHeaderValue);
        });
        serviceCollection.AddSingleton<ITvHeadendApiClient, TvHeadendApiClient>();
        serviceCollection.AddSingleton<ILiveTvService, LiveTvService>();
        serviceCollection.AddSingleton<IChannel, RecordingsChannel>();
    }
}
