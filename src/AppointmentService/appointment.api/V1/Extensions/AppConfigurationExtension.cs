using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace appointment.api.V1.Extensions
{
    public static class AppConfigurationExtension
    {
        public static void AddAzureAppConfigurationWithSecrets(this ConfigurationManager configuration)
        {
            var connectionString = configuration["AppConfiguration:ConnectionString"];
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("AppConfiguration:ConnectionString is missing or empty.");

            if (connectionString.StartsWith("secret://"))
            {
                var parts = connectionString.Replace("secret://", "").Split('/');
                if (parts.Length != 2)
                    throw new ArgumentException("Invalid secret reference format in AppConfiguration:ConnectionString.");

                var vaultName = parts[0];
                var secretName = parts[1];
                var secretClient = new SecretClient(new Uri($"https://{vaultName}.vault.azure.net/"), new DefaultAzureCredential());
                connectionString = secretClient.GetSecret(secretName).Value.Value;
            }

            configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(connectionString)
                       .Select(KeyFilter.Any)
                       .ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()))
                       .UseFeatureFlags();
            });
        }
    }
}
