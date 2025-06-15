using authentication.models.V1.Context;
using authentication.services.V1.Extensions;
using authentication.services.V1.Mapping;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace authentication.api.V1.Extensions;

internal static class ServiceCollectionExtension
{
    internal static void AddServiceCollection(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthorization();
        services.AddApplicationInsightsTelemetry();
        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.UseCredential(new DefaultAzureCredential());
        });
        services.AddDbContext<AuthDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SqlConnection"))
        );
        AddApiVersioning(services);
        AddAzureAppConfigutaion(configuration);
        AddJwtAuthentication(services, configuration);
        services.AddAutoMapper(typeof(AuthMappingProfile));
        services.AddAuthServices();
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

    //private static void AddAzureAppConfigutaion(ConfigurationManager configuration)
    //{
    //    configuration.AddAzureAppConfiguration(options =>
    //    {
    //        options
    //            .Connect(configuration.GetSection("AppConfiguration:ConnectionString").Value)
    //            .UseFeatureFlags();
    //    });
    //}

    private static void AddAzureAppConfigutaion(ConfigurationManager configuration)
    {
        var connectionString = configuration.GetSection("AppConfiguration:ConnectionString").Value;
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("AppConfiguration:ConnectionString is missing or empty.");
        }

        // Handle secret:// reference
        if (connectionString.StartsWith("secret://"))
        {
            // Extract vault and secret name from secret://healthcare-vault/AppConfigConnection
            var parts = connectionString.Replace("secret://", "").Split('/');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid secret reference format in AppConfiguration:ConnectionString.");
            }
            var vaultName = parts[0];
            var secretName = parts[1];
            var vaultUri = $"https://{vaultName}.vault.azure.net/";

            // Use managed identity to retrieve secret
            var secretClient = new SecretClient(new Uri(vaultUri), new DefaultAzureCredential());
            var secret = secretClient.GetSecret(secretName).Value;
            connectionString = secret.Value;
        }

        configuration.AddAzureAppConfiguration(options =>
        {
            options
                .Connect(connectionString)
                .Select(KeyFilter.Any, LabelFilter.Null)
                .ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()))
                .UseFeatureFlags();
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
