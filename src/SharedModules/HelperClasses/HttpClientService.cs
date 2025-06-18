using Microsoft.AspNetCore.Http;
using shared.HelperClasses.Contracts;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace shared.HelperClasses;

public class HttpClientService(HttpClient _httpClient, IHttpContextAccessor _httpContextAccessor) : IHttpClientService
{
    public async Task<HttpResponseMessage> SendAsync<TRequest>(
        HttpMethod method,
        string baseUrl,
        string apiPath,
        TRequest content = default!,
        string bearerToken = null!,
        Dictionary<string, string> headers = null!,
        Dictionary<string, string> queryParams = null!,
        CancellationToken cancellationToken = default)
    {
        var apiPathWithVersion = UrlHelper.GetVersionedApiPath(_httpContextAccessor, apiPath);
        var fullPath = UrlHelper.BuildUrl(baseUrl, apiPathWithVersion, null);
        var request = new HttpRequestMessage(method, fullPath);

        if (content is not null && method != HttpMethod.Get)
        {
            var json = JsonSerializer.Serialize(content);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        if (headers is not null)
        {
            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return await _httpClient.SendAsync(request, cancellationToken);
    }
}
