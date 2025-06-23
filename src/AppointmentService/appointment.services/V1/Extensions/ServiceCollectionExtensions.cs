using appointment.repositories.V1.Contracts;
using appointment.repositories.V1.RepositoryImpl;
using appointment.services.V1.Contracts;
using appointment.services.V1.Exceptions;
using appointment.services.V1.Services;
using Microsoft.Extensions.DependencyInjection;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Extensions;
using shared.V1.HelperClasses.Hubs;

namespace appointment.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAppointmentServices(this IServiceCollection services, ISecretProvider secretProvider)
    {
        services.AddSharedServices();
        services.AddSharedSignalR(secretProvider);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IAppointmentService, AppointmentServiceImpl>();
        services.AddScoped<IExceptionHandlerStrategy, AppointmentExceptionStrategy>();
    }
}
