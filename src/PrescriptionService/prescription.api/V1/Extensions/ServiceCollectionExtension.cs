using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using prescription.api.V1.ModelBinders;
using prescription.models.V1.Dto;
using prescription.repositories.V1.Context;
using prescription.services.V1.Extensions;
using prescription.services.V1.Mappings;
using shared.V1.ModelBinders;
using System.Text;

namespace prescription.api.V1.Extensions;

internal static class ServiceCollectionExtension
{
    internal static void AddServiceCollection(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        services.AddControllers(options =>
        {
            var provider = services
         .BuildServiceProvider()
         .GetRequiredService<IModelBinderProvider>();

            options.ModelBinderProviders.Insert(0, provider);
        });
        services.AddEndpointsApiExplorer();
        services.AddSwagerUi();
        services.AddAuthorization();
        services.AddApplicationInsightsTelemetry();
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.UseCredential(new DefaultAzureCredential());
        });
        services.AddAuthDbContext(configuration);
        services.AddApiVersioning();
        services.AddJwtAuthentication(configuration);
        services.AddAutoMapper(typeof(PrescriptionMappingProfile));
        services.AddHttpClient();
        services.AddPrescriptionServices(configuration);
        services.AddModelBinder();
    }

    private static void AddModelBinder(this IServiceCollection services)
    {
        services.AddTransient<IPropertySetChecker<UpdatePrescriptionRequestDto>, UpdatePrescriptionDtoPropertyChecker>();
        services.AddSingleton<IModelBinderProvider>(sp =>
            new GenericModelBinderProvider<UpdatePrescriptionRequestDto>(sp));
    }

    private static void AddSwagerUi(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Prescription API", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}' below."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
        });
    }

    private static void AddAuthDbContext(this IServiceCollection services, ConfigurationManager configuration)
    {
        var keyVaultUri = configuration["KeyVault:VaultUri"]
                ?? Environment.GetEnvironmentVariable("KeyVault:VaultUri")
                ?? "https://healthcare-vault.vault.azure.net/";
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        var sqlConnectionString = secretClient.GetSecret("SqlConnection").Value.Value;
        services.AddDbContext<PrescriptionDbContext>(options =>
                    options.UseSqlServer(sqlConnectionString)
                    .UseSnakeCaseNamingConvention()
                );

        services.AddHealthChecks()
        .AddDbContextCheck<PrescriptionDbContext>(
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
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                    ),
                };
            });
    }
}
