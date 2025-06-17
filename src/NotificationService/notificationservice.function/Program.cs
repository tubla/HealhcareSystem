using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using notificationservice.function;
using notificationservice.function.Contracts;

//Comment for testing CI/CD pipeline
var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped<IEmailClient, AzureCommunicationEmailClient>();
    })
    .Build();

host.Run();
