using Microsoft.AspNetCore.Http;

namespace shared.HelperClasses.Extensions;

public static class HttpContextExtensions
{
    public static string? GetBearerToken(this IHttpContextAccessor accessor)
    {
        var authHeader = accessor?.HttpContext?.Request?.Headers["Authorization"].ToString();

        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return authHeader["Bearer ".Length..].Trim();
        }

        return null;
    }
}
