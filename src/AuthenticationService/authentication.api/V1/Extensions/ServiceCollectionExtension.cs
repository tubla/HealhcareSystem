using authentication.api.V1.Filters;
using authentication.models.V1.Context;
using authentication.services.V1.Extensions;
using authentication.services.V1.Mapping;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using shared.V1.HelperClasses.Contracts;
using System.Text;

namespace authentication.api.V1.Extensions;

internal static class ServiceCollectionExtension
{
    internal static void AddServiceCollection(
        this IServiceCollection services,
        ISecretProvider secretProvider,
        ConfigurationManager configuration
    )
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwagerUi();
        services.AddAuthorization();
        services.AddApplicationInsightsTelemetry();
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.UseCredential(new DefaultAzureCredential());
        });
        services.AddAuthDbContext(secretProvider, configuration);

        services.AddApiVersioning();
        services.AddJwtAuthentication(secretProvider, configuration);
        services.AddAutoMapper(typeof(AuthMappingProfile));
        services.AddAuthServices();
    }

    private static void AddSwagerUi(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}' below."
            });

            // Apply security requirement selectively using the operation filter
            options.OperationFilter<AuthorizeOperationFilter>();
        });
    }

    private static void AddAuthDbContext(this IServiceCollection services, ISecretProvider secretProvider, ConfigurationManager configuration)
    {
        var sqlConnectionString = secretProvider.GetSecret("SqlConnection");
        services.AddDbContext<AuthDbContext>(options =>
                    options.UseSqlServer(sqlConnectionString, sql => sql.EnableRetryOnFailure(
                                            maxRetryCount: 5,
                                            maxRetryDelay: TimeSpan.FromSeconds(10),
                                            errorNumbersToAdd: null
                                        ))
                           .UseSnakeCaseNamingConvention()
                );

        services.AddHealthChecks()
        .AddDbContextCheck<AuthDbContext>(
        name: "sql-db",
        failureStatus: Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy,
        tags: new[] { "db", "sql", "auth" }
        );
    }

    private static void AddApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
        });
    }

    private static void AddJwtAuthentication(
        this IServiceCollection services,
        ISecretProvider secretProvider,
        ConfigurationManager configuration
    )
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretProvider.GetSecret("JwtKey")!)
                    ),
                };
            });
    }
}
