using Microsoft.AspNetCore.Http;
using shared.HelperClasses.Contracts;
using shared.HelperClasses.Extensions;

namespace shared.HelperClasses;

public class AppointmentServiceProxy(IHttpClientService _httpClientService, IHttpContextAccessor _httpContextAccessor) : IAppointmentServiceProxy
{
    private const string _baseUrl = "http://appointment-service";
    public async Task<bool> CheckAppointmentExistsAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiPath = $"api/v0/appointments/check-appointment-exists";
            var token = _httpContextAccessor.GetBearerToken()!;
            var request = new { AppointmentId = appointmentId };
            return await ExecuteService(_httpClientService, apiPath, request, token, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Appointment service. Please try again later or contact support.");
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
            throw new HttpRequestException($"Appointment Service response failed with status code: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return bool.TryParse(content, out var result) && result;
    }
}
