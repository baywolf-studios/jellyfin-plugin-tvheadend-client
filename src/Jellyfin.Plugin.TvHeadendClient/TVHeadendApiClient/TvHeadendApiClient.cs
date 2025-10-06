using System.Text.Json;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.TvHeadendClient.Http;
using Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient;

public partial class TvHeadendApiClient(IHttpClientFactory httpClientFactory) : ITvHeadendApiClient
{
    public async Task<DvrEntryCreateResponse> CreateDvrEntryAsync(DigestConnectionInfo connectionInfo,
        DvrEntryCreateRequest dvrEntryCreateRequest,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        const string url = "api/dvr/entry/create";

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("conf", JsonSerializer.Serialize(dvrEntryCreateRequest))
        ]);

        return await httpClient
            .PostAndReadAsJsonWithDigestAuthAsync<DvrEntryCreateResponse>(url, content, connectionInfo.Username,
                connectionInfo.Password, cancellationToken)
            .ConfigureAwait(false) ?? new DvrEntryCreateResponse();
    }

    public async Task<DvrEntryCreateByEventResponse> CreateDvrEntryByEventAsync(DigestConnectionInfo connectionInfo,
        string eventId,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        const string url = "api/dvr/entry/create_by_event";

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("config_uuid", string.Empty),
            new KeyValuePair<string, string>("event_id", eventId)
        ]);

        return await httpClient
            .PostAndReadAsJsonWithDigestAuthAsync<DvrEntryCreateByEventResponse>(url, content, connectionInfo.Username,
                connectionInfo.Password, cancellationToken)
            .ConfigureAwait(false) ?? new DvrEntryCreateByEventResponse();
    }

    public async Task<DvrAutorecCreateResponse> CreateDvrAutorecAsync(DigestConnectionInfo connectionInfo,
        DvrAutorecCreateRequest dvrAutorecCreateRequest,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        const string url = "api/dvr/autorec/create";

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("conf", JsonSerializer.Serialize(dvrAutorecCreateRequest))
        ]);

        return await httpClient
            .PostAndReadAsJsonWithDigestAuthAsync<DvrAutorecCreateResponse>(url, content, connectionInfo.Username,
                connectionInfo.Password, cancellationToken)
            .ConfigureAwait(false) ?? new DvrAutorecCreateResponse();
    }

    public async Task<DvrAutorecCreateBySeriesResponse> CreateDvrAutorecBySeriesAsync(
        DigestConnectionInfo connectionInfo, string eventId,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        const string url = "api/dvr/autorec/create_by_series";

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("config_uuid", string.Empty),
            new KeyValuePair<string, string>("event_id", eventId)
        ]);

        return await httpClient
            .PostAndReadAsJsonWithDigestAuthAsync<DvrAutorecCreateBySeriesResponse>(url, content,
                connectionInfo.Username,
                connectionInfo.Password, cancellationToken)
            .ConfigureAwait(false) ?? new DvrAutorecCreateBySeriesResponse();
    }

    public async Task<EpgEventGridResponse> GetEpgEventGridAsync(DigestConnectionInfo connectionInfo,
        string channelId, int limit, CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"api/epg/events/grid?limit={limit}&channel={channelId}";

        return await httpClient.GetAndReadAsJsonWithDigestAuthAsync<EpgEventGridResponse>(url, connectionInfo.Username,
                   connectionInfo.Password,
                   cancellationToken) ??
               new EpgEventGridResponse();
    }

    public async Task<ChannelGridResponse> GetChannelGridAsync(DigestConnectionInfo connectionInfo, int limit,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"api/channel/grid?limit={limit}";

        return await httpClient.GetAndReadAsJsonWithDigestAuthAsync<ChannelGridResponse>(url, connectionInfo.Username,
                   connectionInfo.Password,
                   cancellationToken) ??
               new ChannelGridResponse();
    }

    public async Task<DvrEventGridResponse> GetDvrEventGridAsync(DigestConnectionInfo connectionInfo, int limit,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"api/dvr/entry/grid?limit={limit}";

        return await httpClient.GetAndReadAsJsonWithDigestAuthAsync<DvrEventGridResponse>(url, connectionInfo.Username,
                   connectionInfo.Password,
                   cancellationToken) ??
               new DvrEventGridResponse();
    }

    public async Task<DvrEventGridResponse> GetDvrEventGridUpcomingAsync(DigestConnectionInfo connectionInfo, int limit,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"api/dvr/entry/grid_upcoming?limit={limit}";

        return await httpClient.GetAndReadAsJsonWithDigestAuthAsync<DvrEventGridResponse>(url, connectionInfo.Username,
                   connectionInfo.Password,
                   cancellationToken) ??
               new DvrEventGridResponse();
    }

    public async Task<DvrEventGridResponse> GetDvrEventGridFinishedAsync(DigestConnectionInfo connectionInfo, int limit,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"api/dvr/entry/grid_finished?limit={limit}";

        return await httpClient.GetAndReadAsJsonWithDigestAuthAsync<DvrEventGridResponse>(url, connectionInfo.Username,
                   connectionInfo.Password,
                   cancellationToken) ??
               new DvrEventGridResponse();
    }

    public async Task<DvrAutorecGridResponse> GetDvrAutorecGridAsync(DigestConnectionInfo connectionInfo, int limit,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"api/dvr/autorec/grid?limit={limit}";

        return await httpClient.GetAndReadAsJsonWithDigestAuthAsync<DvrAutorecGridResponse>(url,
                   connectionInfo.Username,
                   connectionInfo.Password,
                   cancellationToken) ??
               new DvrAutorecGridResponse();
    }

    public async Task<DvrConfigGridResponse> GetDvrConfigGridAsync(DigestConnectionInfo connectionInfo, int limit,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"api/dvr/config/grid?limit={limit}";

        return await httpClient.GetAndReadAsJsonWithDigestAuthAsync<DvrConfigGridResponse>(url, connectionInfo.Username,
                   connectionInfo.Password,
                   cancellationToken) ??
               new DvrConfigGridResponse();
    }

    public async Task<ServiceStreamsResponse> GetServiceStreamsAsync(DigestConnectionInfo connectionInfo,
        string serviceId,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"api/service/streams?uuid={serviceId}";

        return await httpClient.GetAndReadAsJsonWithDigestAuthAsync<ServiceStreamsResponse>(url,
                   connectionInfo.Username,
                   connectionInfo.Password,
                   cancellationToken) ??
               new ServiceStreamsResponse();
    }

    public async Task<string> GetPlayUrlForStreamService(DigestConnectionInfo connectionInfo, string serviceId,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"play/ticket/stream/service/{serviceId}";

        var response = await httpClient.GetAndReadAsStringWithDigestAuthAsync(url, connectionInfo.Username,
            connectionInfo.Password, cancellationToken);

        var streamPathMatch = HttpRegex().Match(response ?? string.Empty);
        if (!streamPathMatch.Success)
        {
            throw new InvalidOperationException("Failed to find a valid stream path in the response.");
        }

        return streamPathMatch.Value;
    }

    public async Task<string> GetPlayUrlForDvrFile(DigestConnectionInfo connectionInfo, string dvrFileId,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"play/ticket/dvrfile/{dvrFileId}";

        var response = await httpClient.GetAndReadAsStringWithDigestAuthAsync(url, connectionInfo.Username,
            connectionInfo.Password, cancellationToken);

        var streamPathMatch = HttpRegex().Match(response ?? string.Empty);
        if (!streamPathMatch.Success)
        {
            throw new InvalidOperationException("Failed to find a valid stream path in the response.");
        }

        return streamPathMatch.Value;
    }

    public async Task<EpgEventsLoadResponse> GetEpgEventsLoadAsync(DigestConnectionInfo connectionInfo, string eventId,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(nameof(TvHeadendApiClient));
        httpClient.BaseAddress = connectionInfo.BaseUri;

        var url = $"api/epg/events/load?eventId={eventId}";

        return await httpClient.GetAndReadAsJsonWithDigestAuthAsync<EpgEventsLoadResponse>(url, connectionInfo.Username,
                   connectionInfo.Password,
                   cancellationToken) ??
               new EpgEventsLoadResponse();
    }

    public async Task UpdateDvrEntryAsync(DigestConnectionInfo connectionInfo,
        DvrEntryUpdateRequest dvrEntryCreateRequest,
        CancellationToken cancellationToken)
    {
        await UpdateIdNodeAsync(connectionInfo, dvrEntryCreateRequest, cancellationToken);
    }

    public async Task UpdateDvrAutorecAsync(DigestConnectionInfo connectionInfo,
        DvrAutorecUpdateRequest dvrAutorecCreateRequest,
        CancellationToken cancellationToken)
    {
        await UpdateIdNodeAsync(connectionInfo, dvrAutorecCreateRequest, cancellationToken);
    }

    public async Task CancelDvrEntryAsync(DigestConnectionInfo connectionInfo, string uuid,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        const string url = "api/dvr/entry/cancel";

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("uuid", uuid)
        ]);

        await httpClient.PostWithDigestAuthAsync(url, content, connectionInfo.Username,
            connectionInfo.Password, cancellationToken);
    }

    public async Task RemoveDvrEntryAsync(DigestConnectionInfo connectionInfo, string uuid,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        const string url = "api/dvr/entry/remove";

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("uuid", uuid)
        ]);

        await httpClient.PostWithDigestAuthAsync(url, content, connectionInfo.Username,
            connectionInfo.Password, cancellationToken);
    }

    public async Task CancelDvrAutorecAsync(DigestConnectionInfo connectionInfo, string uuid,
        CancellationToken cancellationToken)
    {
        await DeleteIdNodeAsync(connectionInfo, uuid, cancellationToken);
    }

    public async Task<byte[]?> GetImageAsync(DigestConnectionInfo connectionInfo, string url,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        if (url.StartsWith("imagecache", StringComparison.OrdinalIgnoreCase))
        {
            httpClient.BaseAddress = connectionInfo.BaseUri;
        }

        var image = await httpClient.GetAndReadAsByteArrayWithDigestAuthAsync(url, connectionInfo.Username,
            connectionInfo.Password, cancellationToken);

        return image;
    }

    private async Task UpdateIdNodeAsync<T>(DigestConnectionInfo connectionInfo, T payload,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        const string url = "api/idnode/save";

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("node", JsonSerializer.Serialize(payload))
        ]);

        await httpClient
            .PostWithDigestAuthAsync(url, content, connectionInfo.Username,
                connectionInfo.Password, cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task DeleteIdNodeAsync(DigestConnectionInfo connectionInfo, string uuid,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(PluginInfo.Name);
        httpClient.BaseAddress = connectionInfo.BaseUri;

        const string url = "api/idnode/delete";

        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("uuid", uuid)
        ]);

        await httpClient.PostWithDigestAuthAsync(url, content, connectionInfo.Username,
            connectionInfo.Password, cancellationToken);
    }

    [GeneratedRegex(@"http[^\r\n]+")]
    private static partial Regex HttpRegex();
}
