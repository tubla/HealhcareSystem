using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.SecretClientHelper;

public static class SharedSecretsServiceCollectionExtensions
{
    public static async Task<ISecretProvider> AddSharedSecretsAsync(this IServiceCollection services, IConfiguration configuration)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());

        var keyVaultUri = configuration["KeyVault:VaultUri"]
            ?? Environment.GetEnvironmentVariable("KeyVault:VaultUri")
            ?? "https://healthcare-vault.vault.azure.net/";
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

        string[] secretKeys = ["SignalRConnection",
                               "EventHubConnection",
                               "BlobStorageConnection",
                               "AppConfigConnection",
                               "AppInsightsConnection",
                               "SqlConnection",
                               "JwtKey"];

        foreach (var key in secretKeys)
        {
            var secret = await secretClient.GetSecretAsync(key);
            cache.Set(key, secret.Value.Value);
        }

        var secretProvider = new SecretProvider(cache);

        services.AddSingleton<IMemoryCache>(cache);
        services.AddSingleton<ISecretProvider>(secretProvider);

        return secretProvider;
    }
}
