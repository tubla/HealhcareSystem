using authentication.repositories.V1.Contracts;
using authentication.repositories.V1.RepositoryImpl;
using authentication.services.V1.Contracts;
using authentication.services.V1.CustomExceptions;
using authentication.services.V1.ServiceImpl;
using Microsoft.Extensions.DependencyInjection;
using shared.V1.HelperClasses.Contracts;

namespace authentication.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthServiceImpl>();
        services.AddScoped<IExceptionHandlerStrategy, AuthExceptionStrategy>();
    }
}
