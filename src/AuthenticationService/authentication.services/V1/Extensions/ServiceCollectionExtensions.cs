using authentication.repositories.V1.Contracts;
using authentication.repositories.V1.RepositoryImpl;
using authentication.services.V1.Contracts;
using authentication.services.V1.ServiceImpl;
using Microsoft.Extensions.DependencyInjection;

namespace authentication.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthServiceImpl>();
        return services;
    }
}
