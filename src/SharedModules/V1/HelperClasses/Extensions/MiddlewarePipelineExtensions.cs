using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using shared.V1.HelperClasses.Middlewares;

namespace shared.V1.HelperClasses.Extensions;

public static class MiddlewarePipelineExtensions
{
    public static void UseApplicationMiddlewares(this WebApplication app,
                                                      Action<WebApplication>? preSecurity = null,
                                                      Action<WebApplication>? postSecurity = null)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        preSecurity?.Invoke(app);

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        postSecurity?.Invoke(app);

        app.MapHealthChecks("/healthz");
        app.MapControllers();
    }
}
