using Microsoft.AspNetCore.Http;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Extensions;

namespace shared.V1.HelperClasses.ServiceProxies;

internal class DoctorServiceProxy(IHttpClientService _httpClientService, IHttpContextAccessor _httpContextAccessor) : IDoctorServiceProxy
{
    private const string _baseUrl = "http://doctor-service";
    public async Task<bool> CheckDoctorAssigned(int deptId, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiPath = $"api/v0/doctors/check-department-assigned";
            var token = _httpContextAccessor.GetBearerToken()!;
            var request = new { DeptId = deptId };
            return await ExecuteService(_httpClientService, apiPath, request, token, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Doctor service. Please try again later or contact support.");
        }
    }

    public async Task<bool> CheckDoctorAvailable(int doctorId, DateTime dateTime, CancellationToken cancellationToken)
    {
        try
        {
            var apiPath = $"api/v0/doctors/check-doctor-availability";
            var token = _httpContextAccessor.GetBearerToken()!;
            var request = new { DoctorId = doctorId, AvailableOnDate = dateTime };
            return await ExecuteService(_httpClientService, apiPath, request, token, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Doctor service. Please try again later or contact support.");
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
            throw new HttpRequestException($"Doctor Service response failed with status code: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return bool.TryParse(content, out var result) && result;
    }

}
