using Microsoft.AspNetCore.Http;
using prescription.models.V1.Dto;
using prescription.services.V1.Contracts;
using shared.HelperClasses.Contracts;
using shared.HelperClasses.Extensions;
using shared.Models;
using System.Text.Json;

namespace prescription.services.V1.Services;

public class MediaServiceProxy(IHttpClientService _httpClientService, IHttpContextAccessor _httpContextAccessor) : IMediaServiceProxy
{
    private const string _baseUrl = "http://media-service";

    public async Task<Response<IEnumerable<MediaDto>>> GetAllMediaAsync(IEnumerable<int> mediaIds, CancellationToken cancellationToken)
    {
        try
        {
            var apiPath = $"api/v0/media?mediaIds={string.Join(",", mediaIds)}";
            var token = _httpContextAccessor.GetBearerToken()!;
            var response = await _httpClientService.SendAsync<Response<IEnumerable<MediaDto>>>(
                        HttpMethod.Get,
                        _baseUrl,
                        apiPath,
                        null!,
                        bearerToken: token,
                        cancellationToken: cancellationToken
                    );

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Media Service response failed with status code: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<Response<IEnumerable<MediaDto>>>(content) ?? throw new Exception("Failed to deserialize media data.");
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Media service. Please try again later or contact support.");
        }
    }

    public async Task<Response<MediaDto>> UploadMediaAsync(MultipartFormDataContent content, CancellationToken cancellationToken)
    {
        var apiPath = $"api/v0/media";
        var token = _httpContextAccessor.GetBearerToken()!;
        var request = new { Content = content };
        var response = await _httpClientService.SendAsync(
                    HttpMethod.Post,
                    _baseUrl,
                    apiPath,
                    request,
                    bearerToken: token,
                    cancellationToken: cancellationToken
                );

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Media Service response failed with status code: {response.StatusCode}");

        var resultContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Response<MediaDto>>(resultContent) ?? throw new Exception("Failed to deserialize media data.");
    }
}
