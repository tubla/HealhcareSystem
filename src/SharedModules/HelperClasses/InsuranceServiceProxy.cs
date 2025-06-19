using shared.HelperClasses.Contracts;

namespace shared.HelperClasses;

public class InsuranceServiceProxy(IHttpClientService _httpClientService) : IInsuranceServiceProxy
{
    private const string _baseUrl = "http://insurance-provider-service";
    public async Task<bool> CheckInsuranceProviderAsync(int insuranceProviderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiPath = $"api/v0/insurance/check-insurance-provider";
            var request = new { InsuranceProviderId = insuranceProviderId };
            return await ExecuteService(_httpClientService, apiPath, request, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Insurance Provider service. Please try again later or contact support.");
        }
    }

    private static async Task<bool> ExecuteService(IHttpClientService _httpClientService, string apiPath, object request, CancellationToken cancellationToken)
    {
        var response = await _httpClientService.SendAsync(
                        HttpMethod.Post,
                        _baseUrl,
                        apiPath,
                        request,
                        bearerToken: null!, // or read from context
                        cancellationToken: cancellationToken
                    );

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Auth check failed: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return bool.TryParse(content, out var result) && result;
    }
}
