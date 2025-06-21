using appointment.api.V1.Extensions;
using shared.V1.HelperClasses.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container..
var tempLoggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information);
});

var logger = tempLoggerFactory.CreateLogger("AppConfigurationExtension");

// Load configuration from Azure App Configuration with Key Vault secrets
builder.Configuration.AddAzureAppConfigurationWithSecrets(logger);

builder.Services.AddServiceCollection(builder.Configuration);
var app = builder.Build();
app.UseApplicationMiddlewares();
app.Run();
