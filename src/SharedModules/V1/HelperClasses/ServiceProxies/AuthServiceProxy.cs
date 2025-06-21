using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.ServiceProxies;

internal class AuthServiceProxy(IHttpClientService _httpClientService) : IAuthServiceProxy
{
    private const string _baseUrl = "http://auth-service";
    public async Task<bool> CheckPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiPath = $"api/v0/auth/check-permission";
            var request = new { UserId = userId, PermissionName = permissionName };
            return await ExecuteService(_httpClientService, apiPath, request, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Authentication service. Please try again later or contact support.");
        }
    }

    public async Task<bool> CheckUserExistsAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiPath = $"api/v0/auth/check-user-exists";
            var request = new { UserId = userId };
            return await ExecuteService(_httpClientService, apiPath, request, cancellationToken);
        }
        catch (Exception)
        {
            throw new Exception("Failed to connect to Authentication service. Please try again later or contact support.");
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
            throw new HttpRequestException($"Auth check failed: {response.StatusCode}");

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return bool.TryParse(content, out var result) && result;
    }
}
