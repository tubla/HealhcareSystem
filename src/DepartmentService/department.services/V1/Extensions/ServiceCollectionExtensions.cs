using department.repositories.V1.Contracts;
using department.repositories.V1.RepositoryImpl;
using department.services.V1.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Extensions;

namespace department.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDepartmentServices(this IServiceCollection services)
    {
        services.AddSharedServices();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDepartmentRepository, DepartmentRepositoryImpl>();
        services.AddScoped<IExceptionHandlerStrategy, DepartmentExceptionStrategy>();
    }
}
