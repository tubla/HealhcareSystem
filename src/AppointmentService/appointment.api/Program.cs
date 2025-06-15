using appointment.api.V1.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container..
var tempLoggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddConsole()
        .SetMinimumLevel(LogLevel.Information);
});

var logger = tempLoggerFactory.CreateLogger("AppConfigurationExtension");
builder.Configuration.AddAzureAppConfigurationWithSecrets(logger);
builder.Services.AddServiceCollection(builder.Configuration);
var app = builder.Build();
app.UseApplicationMiddlewares();
app.Run();
