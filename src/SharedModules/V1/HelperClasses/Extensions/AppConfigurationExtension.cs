using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;
using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.Extensions;

public static class AppConfigurationExtension
{
    public static void AddAzureAppConfigurationWithSecrets(this ConfigurationManager configuration, ISecretProvider secretProvider, ILogger logger)
    {
        try
        {
            logger?.LogInformation($"Entering : AddAzureAppConfigurationWithSecrets");

            //var keyVaultUri = configuration["KeyVault:VaultUri"]
            //    ?? Environment.GetEnvironmentVariable("KeyVault:VaultUri")
            //    ?? "https://healthcare-vault.vault.azure.net/";
            // Fetch connection string from Key Vault
            //var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
            //var connectionString = secretClient.GetSecret("AppConfigConnection").Value.Value;
            var connectionString = secretProvider.GetSecret("AppConfigConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Failed to retrieve AppConfiguration connection string from Key Vault.");

            configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(connectionString)
                       .Select(KeyFilter.Any, LabelFilter.Null)
                       .ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()))
                       .UseFeatureFlags();
            });
            logger?.LogInformation("Successfully connected to Azure AppConfiguration.");
            logger?.LogInformation($"Exiting : AddAzureAppConfigurationWithSecrets");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to initialize Azure AppConfiguration with KeyVault URI: {KeyVaultUri}", configuration["KeyVault:VaultUri"]);
            throw;
        }


    }
}
