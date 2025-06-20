using Microsoft.AspNetCore.Http;
using shared.HelperClasses.Contracts;
using shared.HelperClasses.Extensions;

namespace shared.HelperClasses;

public class InsuranceServiceProxy(IHttpClientService _httpClientService, IHttpContextAccessor _httpContextAccessor) : IInsuranceServiceProxy
{
    private const string _baseUrl = "http://insurance-provider-service";
    public async Task<bool> CheckInsuranceProviderAsync(int insuranceProviderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiPath = $"api/v0/insurances/check-insurance-provider";
            var token = _httpContextAccessor.GetBearerToken()!;
            var request = new { InsuranceProviderId = insuranceProviderId };
            return await ExecuteService(_httpClientService, apiPath, request, token, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Insurance Provider service. Please try again later or contact support.");
        }
    }

    private static async Task<bool> ExecuteService(IHttpClientService _httpClientService, string apiPath, object request, string token, CancellationToken cancellationToken)
    {
        var response = await _httpClientService.SendAsync(
                        HttpMethod.Post,
                        _baseUrl,
                        apiPath,
                        request,
                        bearerToken: token,
                        cancellationToken: cancellationToken
                    );

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Auth check failed: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return bool.TryParse(content, out var result) && result;
    }
}
