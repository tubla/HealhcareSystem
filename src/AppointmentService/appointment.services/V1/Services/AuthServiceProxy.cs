using appointment.services.V1.Contracts;
using Microsoft.AspNetCore.Http;
using shared.HelperClasses;
using System.Text;
using System.Text.Json;

namespace appointment.services.V1.Services;

public class AuthServiceProxy(HttpClient _httpClient, IHttpContextAccessor _httpContextAccessor) : IAuthServiceProxy
{
    public async Task<bool> CheckPermissionAsync(int userId, string permissionName)
    {
        try
        {
            var baseUrl = "http://auth-service";
            var apiPath = UrlHelper.GetVersionedApiPath(_httpContextAccessor, $"api/v1/auth/check-permission");
            var apiUrl = UrlHelper.BuildUrl(baseUrl, apiPath, null);

            var requestBody = new { UserId = userId, PermissionName = permissionName };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/check-permission")
            {
                Content = jsonContent,
            };


            var resoonse = await _httpClient.SendAsync(httpRequest);
            if (!resoonse.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Failed to check permission: {resoonse.ReasonPhrase}"
                );
            }
            var responseContent = await resoonse.Content.ReadAsStringAsync();
            return bool.Parse(responseContent);
        }
        catch (Exception)
        {
            return false; // Fallback if Auth Service is unavailable
        }
    }
}
