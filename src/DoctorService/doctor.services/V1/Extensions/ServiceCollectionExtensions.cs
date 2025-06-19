using doctor.repositories.V1.Contracts;
using doctor.repositories.V1.RepositoryImpl;
using doctor.services.V1.Contracts;
using doctor.services.V1.Services;
using Microsoft.Extensions.DependencyInjection;
using shared.HelperClasses;
using shared.HelperClasses.Contracts;

namespace doctor.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDoctorServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDoctorRepository, DoctorRepositoryImpl>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAuthServiceProxy, AuthServiceProxy>();
        services.AddScoped<IAppointmentServiceProxy, AppointmentServiceProxy>();
        services.AddScoped<IDepartmentServiceProxy, DepartmentServiceProxy>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddMemoryCache();
    }
}
