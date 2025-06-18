using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace patient.api.V1.Extensions;

public static class AppConfigurationExtension
{
    public static void AddAzureAppConfigurationWithSecrets(this ConfigurationManager configuration, ILogger logger)
    {
        try
        {
            logger?.LogInformation($"Entering : AddAzureAppConfigurationWithSecrets");

            var keyVaultUri = configuration["KeyVault:VaultUri"]
                ?? Environment.GetEnvironmentVariable("KeyVault:VaultUri")
                ?? "https://healthcare-vault.vault.azure.net/";
            // Fetch connection string from Key Vault
            var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
            var connectionString = secretClient.GetSecret("AppConfigConnection").Value.Value;

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
