using department.repositories.V1.Contracts;
using department.repositories.V1.RepositoryImpl;
using Microsoft.Extensions.DependencyInjection;
using shared.HelperClasses;
using shared.HelperClasses.Contracts;

namespace department.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDepartmentServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDepartmentRepository, DepartmentRepositoryImpl>();
        services.AddScoped<IDoctorServiceProxy, DoctorServiceProxy>();
        services.AddScoped<IAuthServiceProxy, AuthServiceProxy>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddMemoryCache();
    }
}
