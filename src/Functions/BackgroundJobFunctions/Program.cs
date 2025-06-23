using BackgroundJobFunctions.V1.Appointment;
using BackgroundJobFunctions.V1.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using shared.V1.HelperClasses.Extensions;
using shared.V1.HelperClasses.SecretClientHelper;

var builder = new HostApplicationBuilder(args);
var secretProvider = await builder.Services.AddSharedSecretsAsync(builder.Configuration);

var eventHubConn = secretProvider.GetSecret("EventHubConnection");
var sqlConn = secretProvider.GetSecret("SqlConnection");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
{
    { "EventHubConnection", eventHubConn! },
    { "SqlConnection", sqlConn!},
    { "AzureBlobStorage:ContainerName", "media" }
}!);

builder.Services.AddSharedServices();
builder.Services.AddScoped<IEmailClient, AzureCommunicationEmailClient>();

builder.Build().Run();

