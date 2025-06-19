using shared.HelperClasses.Contracts;

namespace shared.HelperClasses;

public class DoctorServiceProxy(IHttpClientService _httpClientService) : IDoctorServiceProxy
{
    private const string _baseUrl = "http://doctor-service";
    public async Task<bool> CheckDoctorAssigned(int deptId, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiPath = $"api/v0/doctor/check-department-assigned";
            var request = new { DeptId = deptId };
            return await ExecuteService(_httpClientService, apiPath, request, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Doctor service. Please try again later or contact support.");
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
            throw new HttpRequestException($"Doctor Service response failed with status code: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return bool.TryParse(content, out var result) && result;
    }
}
