using BackgroundJobFunctions.V1.Appointment;
using BackgroundJobFunctions.V1.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Extensions;

var builder = new HostApplicationBuilder(args);

// Add shared services temporarily so we can resolve ISecretClient
var tempServices = new ServiceCollection();
tempServices.AddSharedServices(); // This includes HealthcareSecretClient

var tempProvider = tempServices.BuildServiceProvider();
var secretClient = tempProvider.GetRequiredService<ISecretClient>();

// Load secrets synchronously at startup and inject them into configuration
var eventHubConn = secretClient.GetSecretValueAsync("EventHubConnection").GetAwaiter().GetResult();

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
{
    { "EventHubConnection", eventHubConn },
    { "AzureBlobStorage:ContainerName", "media" }
}!);

// Register real services for the app, now that config is complete
builder.Services.AddMemoryCache();
builder.Services.AddSharedServices(); // Registers HealthcareSecretClient, warmup services, etc.
builder.Services.AddScoped<IEmailClient, AzureCommunicationEmailClient>();

builder.Build().Run();

