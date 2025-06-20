using appointment.repositories.V1.Contracts;
using appointment.repositories.V1.RepositoryImpl;
using appointment.services.V1.Contracts;
using appointment.services.V1.Services;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shared.HelperClasses;
using shared.HelperClasses.Contracts;

namespace appointment.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAppointmentServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IAppointmentService, AppointmentServiceImpl>();
        services.AddScoped<IAuthServiceProxy, AuthServiceProxy>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddMemoryCache();
        services.AddHttpContextAccessor();
        services.AddSingleton(sp =>
        {
            var connectionString = configuration["EventHubConnection"];
            return new EventHubProducerClient(connectionString, "appointment-scheduled");
        });
    }
}
