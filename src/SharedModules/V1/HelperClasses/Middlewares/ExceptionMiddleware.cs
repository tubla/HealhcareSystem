using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using shared.V1.HelperClasses.Contracts;
using System.Net;
using System.Text.Json;

namespace shared.V1.HelperClasses.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var strategy = context.RequestServices.GetService<IExceptionHandlerStrategy>();
            await HandleExceptionAsync(context, ex, strategy);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, IExceptionHandlerStrategy? strategy)
    {
        var (statusCode, message) = strategy?.TryMap(exception)
            ?? (HttpStatusCode.InternalServerError, "An unexpected error occurred.");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = JsonSerializer.Serialize(new { error = message });
        return context.Response.WriteAsync(response);
    }
}

