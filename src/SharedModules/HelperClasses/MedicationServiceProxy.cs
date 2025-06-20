using Microsoft.AspNetCore.Http;
using shared.HelperClasses.Contracts;
using shared.HelperClasses.Extensions;

namespace shared.HelperClasses;

public class MedicationServiceProxy(IHttpClientService _httpClientService, IHttpContextAccessor _httpContextAccessor) : IMedicationServiceProxy
{
    private const string _baseUrl = "http://medication-service";
    public async Task<bool> CheckMedicationExistsAsync(int medicationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiPath = $"api/v0/medications/check-medication-exists";
            var token = _httpContextAccessor.GetBearerToken()!;
            var request = new { MedicationId = medicationId };
            return await ExecuteService(_httpClientService, apiPath, request, token, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Medication service. Please try again later or contact support.");
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
            throw new HttpRequestException($"Medication Service response failed with status code: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return bool.TryParse(content, out var result) && result;
    }
}
