using Azure.Identity;
using doctor.api.V1.Extensions;
using doctor.api.V1.ModelBinders;
using doctor.models.V1.Dto;
using doctor.repositories.V1.Context;
using doctor.services.V1.Extensions;
using doctor.services.V1.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using shared.V1.HelperClasses.Contracts;
using shared.V1.ModelBinders;
using System.Text;

namespace doctor.api.V1.Extensions;

internal static class ServiceCollectionExtension
{
    internal static void AddServiceCollection(
        this IServiceCollection services,
        ISecretProvider secretProvider,
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
        services.AddDoctorDbContext(secretProvider, configuration);
        services.AddApiVersioning();
        services.AddJwtAuthentication(secretProvider, configuration);
        services.AddAutoMapper(typeof(DoctorMappingProfile));
        services.AddHttpClient();
        services.AddDoctorServices();
        services.AddModelBinder();
    }

    private static void AddModelBinder(this IServiceCollection services)
    {
        services.AddTransient<IPropertySetChecker<UpdateDoctorRequestDto>, UpdateDoctorDtoPropertyChecker>();
        services.AddSingleton<IModelBinderProvider>(sp =>
            new GenericModelBinderProvider<UpdateDoctorRequestDto>(sp));
    }

    private static void AddSwagerUi(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Doctor API", Version = "v1" });
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

    private static void AddDoctorDbContext(this IServiceCollection services, ISecretProvider secretProvider, ConfigurationManager configuration)
    {
        var sqlConnectionString = secretProvider.GetSecret("SqlConnection");
        services.AddDbContext<DoctorDbContext>(options =>
                    options.UseSqlServer(sqlConnectionString)
                    .UseSnakeCaseNamingConvention()
                );

        services.AddHealthChecks()
        .AddDbContextCheck<DoctorDbContext>(
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
