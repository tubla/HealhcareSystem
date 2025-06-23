using media.repositories.V1.Contracts;
using media.repositories.V1.Repositories;
using Microsoft.Extensions.DependencyInjection;
using shared.V1.HelperClasses.Extensions;

namespace media.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMediaServices(this IServiceCollection services)
    {
        services.AddSharedServices();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMediaRepository, MediaRepositoryImpl>();
    }
}
