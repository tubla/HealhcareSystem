using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace appointment.api.V1.Extensions
{
    public static class AppConfigurationExtension
    {
        public static void AddAzureAppConfigurationWithSecrets(this ConfigurationManager configuration, ILogger<AppConfigurationExtension> logger)
        {
            logger.LogInformation($"Entering : AddAzureAppConfigurationWithSecrets");
            // Fetch connection string from Key Vault
            logger.LogInformation($"KeyVault Url : {configuration["KeyVaultUrl"]!}");
            var secretClient = new SecretClient(new Uri(configuration["KeyVaultUrl"]!), new DefaultAzureCredential());
            var connectionString = secretClient.GetSecret("AppConfigConnection").Value.Value;
            logger.LogInformation($"AppConfig Connection String : {connectionString}");
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Failed to retrieve AppConfiguration connection string from Key Vault.");

            configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(connectionString)
                       .Select(KeyFilter.Any, LabelFilter.Null)
                       .ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()))
                       .UseFeatureFlags();
            });
            logger.LogInformation($"Exiting : AddAzureAppConfigurationWithSecrets");
        }
    }
}
