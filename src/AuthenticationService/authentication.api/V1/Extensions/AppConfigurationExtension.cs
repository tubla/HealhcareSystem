using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace authentication.api.V1.Extensions
{
    public static class AppConfigurationExtension
    {
        public static void AddAzureAppConfigurationWithSecrets(this ConfigurationManager configuration)
        {
            // Fetch connection string from Key Vault
            var secretClient = new SecretClient(new Uri(configuration["KeyVault:VaultUri"]!), new DefaultAzureCredential());
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
        }
    }
}
