using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using prescription.repositories.V1.Contracts;
using prescription.repositories.V1.Repositories;
using prescription.services.V1.Contracts;
using prescription.services.V1.Services;
using shared.HelperClasses;
using shared.HelperClasses.Contracts;

namespace prescription.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPrescriptionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPrescriptionRepository, PrescriptionRepositoryImpl>();
        services.AddScoped<IMediaServiceProxy, MediaServiceProxy>();
        services.AddScoped<IAppointmentServiceProxy, AppointmentServiceProxy>();
        services.AddScoped<IAuthServiceProxy, AuthServiceProxy>();
        services.AddScoped<IPrescriptionService, PrescriptionServiceImpl>();
        services.AddScoped<IMedicationServiceProxy, MedicationServiceProxy>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddMemoryCache();
        services.AddSingleton(sp =>
        {
            var connectionString = configuration["EventHubConnection"];
            return new EventHubProducerClient(connectionString, "prescription-events");
        });
    }
}
