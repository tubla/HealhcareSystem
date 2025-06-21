using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using prescription.repositories.V1.Contracts;
using prescription.repositories.V1.Repositories;
using prescription.services.V1.Contracts;
using prescription.services.V1.Services;
using shared.V1.HelperClasses.Extensions;

namespace prescription.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPrescriptionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSharedServices();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPrescriptionRepository, PrescriptionRepositoryImpl>();
        services.AddScoped<IMediaServiceProxy, MediaServiceProxy>();
        services.AddScoped<IPrescriptionService, PrescriptionServiceImpl>();
        services.AddMemoryCache();
    }
}
