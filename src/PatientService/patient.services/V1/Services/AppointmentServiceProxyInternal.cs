using Microsoft.AspNetCore.Http;
using patient.models.V1.Dto;
using patient.services.V1.Contracts;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Extensions;
using System.Text.Json;

namespace doctor.services.V1.Services;

internal class AppointmentServiceProxyInternal(IHttpClientService _httpClientService, IHttpContextAccessor _httpContextAccessor) : IAppointmentServiceProxyInternal
{
    public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var baseUrl = "http://appointment-service";
            var apiPath = $"api/v1/appointments/doctor/{patientId}";
            var token = _httpContextAccessor.GetBearerToken();
            var response = await _httpClientService.SendAsync<IEnumerable<AppointmentResponseDto>>(
                HttpMethod.Get,
                baseUrl,
                apiPath,
                content: null!,
                bearerToken: token!, // or read from context
                cancellationToken: cancellationToken
            );

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"GET request from permission api failed: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<IEnumerable<AppointmentResponseDto>>(content) ??
                throw new JsonException("Unable to deserialize appointment object");
        }
        catch (Exception)
        {
            throw new Exception("Failed to retrieve appointments. Please try again later or contact support.");
        }
    }
}
