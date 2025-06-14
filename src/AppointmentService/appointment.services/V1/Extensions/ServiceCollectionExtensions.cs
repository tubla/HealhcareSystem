using appointment.repositories.V1.Contracts;
using appointment.repositories.V1.RepositoryImpl;
using appointment.services.V1.Contracts;
using appointment.services.V1.Services;
using Microsoft.Extensions.DependencyInjection;

namespace appointment.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppointmentServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IAppointmentService, AppointmentServiceImpl>();
        services.AddScoped<IAuthServiceProxy, AuthServiceProxy>();
        services.AddMemoryCache();
        return services;
    }
}
