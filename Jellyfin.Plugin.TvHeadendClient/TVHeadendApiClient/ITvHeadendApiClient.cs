using Jellyfin.Plugin.TvHeadendClient.Http;
using Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient.Models;

namespace Jellyfin.Plugin.TvHeadendClient.TVHeadendApiClient;

public interface ITvHeadendApiClient
{
    Task<DvrEntryCreateResponse> CreateDvrEntryAsync(DigestConnectionInfo connectionInfo,
        DvrEntryCreateRequest dvrEntryCreateRequest, CancellationToken cancellationToken = default);

    Task<DvrEntryCreateByEventResponse> CreateDvrEntryByEventAsync(DigestConnectionInfo connectionInfo, string eventId,
        CancellationToken cancellationToken = default);

    Task<DvrAutorecCreateResponse> CreateDvrAutorecAsync(DigestConnectionInfo connectionInfo,
        DvrAutorecCreateRequest dvrAutorecCreateRequest,
        CancellationToken cancellationToken = default);

    Task<DvrAutorecCreateBySeriesResponse> CreateDvrAutorecBySeriesAsync(DigestConnectionInfo connectionInfo,
        string eventId, CancellationToken cancellationToken = default);

    Task<ChannelGridResponse> GetChannelGridAsync(DigestConnectionInfo connectionInfo, int limit = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task<EpgEventGridResponse> GetEpgEventGridAsync(DigestConnectionInfo connectionInfo, string channelId,
        int limit = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task<DvrEventGridResponse> GetDvrEventGridAsync(DigestConnectionInfo connectionInfo, int limit = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task<DvrEventGridResponse> GetDvrEventGridUpcomingAsync(DigestConnectionInfo connectionInfo,
        int limit = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task<DvrEventGridResponse> GetDvrEventGridFinishedAsync(DigestConnectionInfo connectionInfo,
        int limit = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task<DvrAutorecGridResponse> GetDvrAutorecGridAsync(DigestConnectionInfo connectionInfo, int limit = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task<DvrConfigGridResponse> GetDvrConfigGridAsync(DigestConnectionInfo connectionInfo, int limit = int.MaxValue,
        CancellationToken cancellationToken = default);

    Task<ServiceStreamsResponse> GetServiceStreamsAsync(DigestConnectionInfo connectionInfo, string serviceId,
        CancellationToken cancellationToken = default);

    Task<string> GetPlayUrlForStreamService(DigestConnectionInfo connectionInfo, string serviceId,
        CancellationToken cancellationToken = default);

    Task<string> GetPlayUrlForDvrFile(DigestConnectionInfo connectionInfo, string dvrFileId,
        CancellationToken cancellationToken = default);

    Task<EpgEventsLoadResponse> GetEpgEventsLoadAsync(DigestConnectionInfo connectionInfo, string eventId,
        CancellationToken cancellationToken = default);

    Task<byte[]?> GetImageAsync(DigestConnectionInfo connectionInfo, string url,
        CancellationToken cancellationToken = default);

    Task UpdateDvrEntryAsync(DigestConnectionInfo connectionInfo,
        DvrEntryUpdateRequest dvrEntryCreateRequest, CancellationToken cancellationToken = default);

    Task UpdateDvrAutorecAsync(DigestConnectionInfo connectionInfo,
        DvrAutorecUpdateRequest dvrAutorecCreateRequest,
        CancellationToken cancellationToken = default);

    Task CancelDvrEntryAsync(DigestConnectionInfo connectionInfo, string uuid,
        CancellationToken cancellationToken = default);

    Task RemoveDvrEntryAsync(DigestConnectionInfo connectionInfo, string uuid,
        CancellationToken cancellationToken = default);

    Task CancelDvrAutorecAsync(DigestConnectionInfo connectionInfo, string uuid,
        CancellationToken cancellationToken = default);
}
