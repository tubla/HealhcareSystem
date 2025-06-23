using Microsoft.Extensions.DependencyInjection;
using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.Hubs;

public static class SignalRConfigurationExtension
{
    public static void AddSharedSignalR(this IServiceCollection services, ISecretProvider secretProvider)
    {
        var connection = secretProvider.GetSecret("SignalRConnection");
        services.AddSignalR().AddAzureSignalR((options) =>
        {
            options.ConnectionString = connection;
        });
    }
}
