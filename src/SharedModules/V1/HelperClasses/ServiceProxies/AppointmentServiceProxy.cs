using Microsoft.AspNetCore.Http;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Extensions;

namespace shared.V1.HelperClasses.ServiceProxies;

internal class AppointmentServiceProxy(IHttpClientService _httpClientService, IHttpContextAccessor _httpContextAccessor) : IAppointmentServiceProxy
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

    public async Task<bool> CheckDoctorHasAppointmentsAsync(int doctorId, CancellationToken cancellationToken)
    {
        try
        {
            var apiPath = $"api/v0/appointments/check-doctor-appointment-exists";
            var token = _httpContextAccessor.GetBearerToken()!;
            var request = new { DoctorId = doctorId };
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
