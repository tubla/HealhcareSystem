using shared.HelperClasses.Contracts;

namespace shared.HelperClasses;

public class AuthServiceProxy(IHttpClientService _httpClientService) : IAuthServiceProxy
{
    public async Task<bool> CheckPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default)
    {
        try
        {
            var baseUrl = "http://auth-service";
            var apiPath = $"api/v0/auth/check-permission";
            var request = new { UserId = userId, PermissionName = permissionName };

            var response = await _httpClientService.SendAsync(
                HttpMethod.Post,
                baseUrl,
                apiPath,
                request,
                bearerToken: null!, // or read from context
                cancellationToken: cancellationToken
            );

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Auth check failed: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return bool.TryParse(content, out var result) && result;
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Authentication service. Please try again later or contact support.");
        }
    }
}
