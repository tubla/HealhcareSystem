using media.api.V1.Extensions;
using shared.V1.HelperClasses.Extensions;
using shared.V1.HelperClasses.SecretClientHelper;

var builder = WebApplication.CreateBuilder(args);

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole().SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger("AppConfigurationExtension");

// Preload secrets and get the actual IMemoryCache instance
var secretProvider = await builder.Services.AddSharedSecretsAsync(builder.Configuration);

// Use the secret for Azure App Configuration
builder.Configuration.AddAzureAppConfigurationWithSecrets(secretProvider, logger);

// Now register additional services
builder.Services.AddServiceCollection(secretProvider, builder.Configuration);

// Build the app — now services are wired correctly
var app = builder.Build();

app.UseApplicationMiddlewares();
app.Run();