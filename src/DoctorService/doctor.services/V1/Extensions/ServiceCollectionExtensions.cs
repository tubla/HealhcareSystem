using doctor.repositories.V1.Contracts;
using doctor.repositories.V1.RepositoryImpl;
using doctor.services.V1.Contracts;
using doctor.services.V1.Exceptions;
using doctor.services.V1.Services;
using Microsoft.Extensions.DependencyInjection;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Extensions;

namespace doctor.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDoctorServices(this IServiceCollection services)
    {
        services.AddSharedServices();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDoctorRepository, DoctorRepositoryImpl>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAppointmentServiceProxyInternal, AppointmentServiceProxyInternal>();
        services.AddScoped<IDepartmentServiceProxy, DepartmentServiceProxy>();
        services.AddScoped<IExceptionHandlerStrategy, DoctorExceptionStrategy>();
    }
}
