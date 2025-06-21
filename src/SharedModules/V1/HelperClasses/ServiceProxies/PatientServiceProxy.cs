using Microsoft.AspNetCore.Http;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Extensions;

namespace shared.V1.HelperClasses.ServiceProxies;

internal class PatientServiceProxy(IHttpClientService _httpClientService, IHttpContextAccessor _httpContextAccessor) : IPatientServiceProxy
{
    private const string _baseUrl = "http://patient-service";
    public async Task<bool> CheckPatientExistsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiPath = $"api/v0/patients/check-patient-exists";
            var token = _httpContextAccessor.GetBearerToken()!;
            var request = new { PatientId = patientId };
            return await ExecuteService(_httpClientService, apiPath, request, token, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Patient service. Please try again later or contact support.");
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
            throw new HttpRequestException($"Patient Service response failed with status code: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return bool.TryParse(content, out var result) && result;
    }
}
