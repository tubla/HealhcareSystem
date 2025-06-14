using System.Net;
using System.Text.Json;
using authentication.services.V1.CustomExceptions;

namespace authentication.api.V1.Middlewares;

public class ExceptionMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";

        switch (exception)
        {
            case ConflictException:
                statusCode = HttpStatusCode.Conflict;
                message = exception.Message;
                break;
            case services.V1.CustomExceptions.UnauthorizedAccessException:
                statusCode = HttpStatusCode.Forbidden;
                message = exception.Message;
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        var response = new { error = message };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
