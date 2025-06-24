using appointment.api.V1.Extensions;
using shared.V1.HelperClasses.Extensions;
using shared.V1.HelperClasses.Hubs;
using shared.V1.HelperClasses.SecretClientHelper;

var builder = WebApplication.CreateBuilder(args);

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole().SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger("AppConfigurationExtension");

// Detect if Swagger CLI is running
var isSwaggerGeneration = args.Contains("tofile");

// Conditionally skip Azure setup
if (!isSwaggerGeneration)
{
    // Preload secrets and get the actual IMemoryCache instance
    var secretProvider = await builder.Services.AddSharedSecretsAsync(builder.Configuration);

    // Use the secret for Azure App Configuration
    builder.Configuration.AddAzureAppConfigurationWithSecrets(secretProvider, logger);

    // Now register additional services
    builder.Services.AddServiceCollection(secretProvider, builder.Configuration);
}
else
{
    // Minimal service setup to satisfy app build during Swagger generation
    builder.Services.AddControllers();
    // (Or any other minimum needed to let app build succeed)
}

// Build the app
var app = builder.Build();

// Use middlewares and run
app.UseApplicationMiddlewares(null, ps =>
{
    ps.MapHub<AppointmentHub>("/appointmentHub");
});

app.Run();
