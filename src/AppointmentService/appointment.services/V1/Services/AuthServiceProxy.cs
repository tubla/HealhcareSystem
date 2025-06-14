using System.Text;
using System.Text.Json;
using appointment.services.V1.Contracts;
using Dapr.Client;

namespace appointment.services.V1.Services;

public class AuthServiceProxy(DaprClient _daprClient) : IAuthServiceProxy
{
    public async Task<bool> CheckPermissionAsync(int userId, string permissionName)
    {
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

        try
        {
            return await _daprClient.InvokeMethodAsync<bool>(httpRequest);
        }
        catch (Dapr.Client.InvocationException)
        {
            return false; // Fallback if Auth Service is unavailable
        }
    }
}
