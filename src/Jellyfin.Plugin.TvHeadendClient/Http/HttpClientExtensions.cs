using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Jellyfin.Plugin.TvHeadendClient.Http.JsonConverters;

namespace Jellyfin.Plugin.TvHeadendClient.Http;

public static partial class HttpClientExtensions
{
    private const string NonceCount = "00000001";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, Converters = { new IntToBoolConverter(), new IntToStringConverter() } };

    private static readonly DigestChallenge DefaultDigestChallenge =
        new(string.Empty, string.Empty, string.Empty, string.Empty);

    public static async Task<byte[]> GetAndReadAsByteArrayWithDigestAuthAsync(this HttpClient httpClient, string url,
        string? username = null,
        string? password = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetWithDigestAuthAsync(url, username, password, cancellationToken)
            .ConfigureAwait(false);
        return await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<byte[]> PostAndReadAsByteArrayWithDigestAuthAsync(this HttpClient httpClient, string url,
        HttpContent? content,
        string? username = null,
        string? password = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostWithDigestAuthAsync(url, content, username, password, cancellationToken)
            .ConfigureAwait(false);
        return await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<T?> GetAndReadAsJsonWithDigestAuthAsync<T>(this HttpClient httpClient, string url,
        string? username = null,
        string? password = null,
        CancellationToken cancellationToken = default)
    {
        var result = await httpClient.GetAndReadAsStringWithDigestAuthAsync(url, username, password, cancellationToken)
            .ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(result, JsonSerializerOptions);
    }

    public static async Task<T?> PostAndReadAsJsonWithDigestAuthAsync<T>(this HttpClient httpClient, string url,
        HttpContent? content,
        string? username = null,
        string? password = null,
        CancellationToken cancellationToken = default)
    {
        var result = await httpClient
            .PostAndReadAsStringWithDigestAuthAsync(url, content, username, password, cancellationToken)
            .ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(result, JsonSerializerOptions);
    }

    public static async Task<string> GetAndReadAsStringWithDigestAuthAsync(this HttpClient httpClient, string url,
        string? username = null,
        string? password = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetWithDigestAuthAsync(url, username, password, cancellationToken)
            .ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<string> PostAndReadAsStringWithDigestAuthAsync(this HttpClient httpClient, string url,
        HttpContent? content,
        string? username = null,
        string? password = null,
        CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostWithDigestAuthAsync(url, content, username, password, cancellationToken)
            .ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<HttpResponseMessage> GetWithDigestAuthAsync(this HttpClient client, string url,
        string? username = null,
        string? password = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendWithDigestAuthAsync(client, request, username, password, cancellationToken);
    }

    public static async Task<HttpResponseMessage> PostWithDigestAuthAsync(this HttpClient httpClient, string url,
        HttpContent? content, string? username = null, string? password = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        return await SendWithDigestAuthAsync(httpClient, request, username, password, cancellationToken);
    }

    public static async Task<HttpResponseMessage> SendWithDigestAuthAsync(
        this HttpClient httpClient, HttpRequestMessage request, string? username = null, string? password = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return response;
        }

        var initialResponse = await httpClient.SendAsync(request, cancellationToken);

        if (initialResponse.StatusCode != HttpStatusCode.Unauthorized ||
            !TryExtractDigestChallenge(initialResponse.Headers, out var digestChallenge))
        {
            initialResponse.EnsureSuccessStatusCode();
            return initialResponse;
        }

        var authenticatedRequest = await CloneHttpRequestMessageAsync(request);
        authenticatedRequest.Headers.Authorization = GetDigestAuthenticationHeader(digestChallenge, username, password,
            request.RequestUri?.PathAndQuery ?? string.Empty, request.Method);

        var authenticatedResponse = await httpClient.SendAsync(authenticatedRequest, cancellationToken);
        authenticatedResponse.EnsureSuccessStatusCode();
        return authenticatedResponse;
    }

    private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
    {
        var clone = new HttpRequestMessage(req.Method, req.RequestUri);

        if (req.Content != null)
        {
            var ms = new MemoryStream();
            await req.Content.CopyToAsync(ms).ConfigureAwait(false);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);

            foreach (var h in req.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(h.Key, h.Value);
            }
        }

        clone.Version = req.Version;

        foreach (var option in req.Options)
        {
            clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);
        }

        foreach (var header in req.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }

    private static bool TryExtractDigestChallenge(HttpResponseHeaders httpResponseHeaders,
        out DigestChallenge digestChallenge)
    {
        if (!httpResponseHeaders.TryGetValues("WWW-Authenticate", out var challengeHeaders))
        {
            digestChallenge = DefaultDigestChallenge;
            return false;
        }

        var digestHeader =
            challengeHeaders.FirstOrDefault(h => h.StartsWith("Digest", StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(digestHeader))
        {
            digestChallenge = DefaultDigestChallenge;
            return false;
        }

        var digestChallengeValues = DigestChallengeRegex().Matches(digestHeader)
            .ToDictionary(c => c.Groups[1].Value, c => c.Groups[2].Value);

        if (!digestChallengeValues.TryGetValue("realm", out var realm) ||
            !digestChallengeValues.TryGetValue("nonce", out var nonce) ||
            !digestChallengeValues.TryGetValue("qop", out var qop) ||
            !digestChallengeValues.TryGetValue("opaque", out var opaque))
        {
            digestChallenge = DefaultDigestChallenge;
            return false;
        }

        var preferredQop = qop.Split(',').Select(s => s.Trim().ToLowerInvariant()).FirstOrDefault(s => s == "auth") ??
                           "auth";

        digestChallenge = new DigestChallenge(realm, nonce, preferredQop, opaque);
        return true;
    }


    private static string Md5Hash(string input)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static AuthenticationHeaderValue GetDigestAuthenticationHeader(
        DigestChallenge digestChallenge, string username, string password, string uri, HttpMethod method)
    {
        var clientNonce = Guid.NewGuid().ToString("N")[..32];

        var ha1 = Md5Hash($"{username}:{digestChallenge.Realm}:{password}");

        var ha2 = Md5Hash($"{method}:{uri}");

        var response = Md5Hash(
            $"{ha1}:{digestChallenge.Nonce}:{NonceCount}:{clientNonce}:{digestChallenge.Qop}:{ha2}");

        var headerValue =
            $"username=\"{username}\", realm=\"{digestChallenge.Realm}\", nonce=\"{digestChallenge.Nonce}\", uri=\"{uri}\", " +
            $"algorithm=MD5, response=\"{response}\", qop={digestChallenge.Qop}, nc={NonceCount}, cnonce=\"{clientNonce}\", opaque=\"{digestChallenge.Opaque}\"";

        return new AuthenticationHeaderValue("Digest", headerValue);
    }

    [GeneratedRegex("""(\w+)=[""]?([^,""']*)[""]?""")]
    private static partial Regex DigestChallengeRegex();

    private record DigestChallenge(string Realm, string Nonce, string Qop, string Opaque);
}
