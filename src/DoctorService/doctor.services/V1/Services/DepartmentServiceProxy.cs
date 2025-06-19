using doctor.models.V1.Dto;
using doctor.services.V1.Contracts;
using Microsoft.AspNetCore.Http;
using shared.HelperClasses.Contracts;
using shared.HelperClasses.Extensions;
using System.Text.Json;

namespace doctor.services.V1.Services;

internal class DepartmentServiceProxy(IHttpClientService _httpClientService, IHttpContextAccessor _httpContextAccessor) : IDepartmentServiceProxy
{
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentAsync(int deptId, CancellationToken cancellationToken = default)
    {
        try
        {
            var baseUrl = "http://department-service";
            var apiPath = $"api/v1/departments/{deptId}";
            var token = _httpContextAccessor.GetBearerToken();
            var response = await _httpClientService.SendAsync<IEnumerable<AppointmentDto>>(
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
            return JsonSerializer.Deserialize<IEnumerable<DepartmentDto>>(content) ??
                throw new JsonException("Unable to deserialize appointment object");
        }
        catch (Exception)
        {
            throw new Exception("Failed to retrieve appointments. Please try again later or contact support.");
        }
    }
}
