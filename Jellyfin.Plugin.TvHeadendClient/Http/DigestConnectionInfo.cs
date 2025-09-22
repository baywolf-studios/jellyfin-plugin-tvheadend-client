namespace Jellyfin.Plugin.TvHeadendClient.Http;

public record DigestConnectionInfo
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public bool UseSsl { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }

    public string Scheme => UseSsl ? "https" : "http";

    public string BaseUrl => $"{Scheme}://{Host}:{Port}";

    public Uri BaseUri => new(BaseUrl);
}