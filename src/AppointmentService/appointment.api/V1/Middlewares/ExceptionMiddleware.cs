using System.Net;
using System.Text.Json;
using appointment.services.V1.Exceptions;

namespace appointment.api.V1.Middlewares;

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
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case AppointmentConflictException:
                statusCode = HttpStatusCode.Conflict;
                message = exception.Message;
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Forbidden;
                message = exception.Message;
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        var response = new { error = message };
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
