using Azure.Identity;
using Azure.Messaging.EventHubs.Producer;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.EventHubs;

// EventHubClientFactory is singleton and ISecretClient is scoped, so we use a delegate function
// to create the ISecretClient instance.
public class EventHubClientFactory(IConfiguration _configuration) : IEventHubClientFactory
{
    public async Task<EventHubProducerClient> CreateAsync(string eventName)
    {
        var keyVaultUri = _configuration["KeyVault:VaultUri"]
             ?? Environment.GetEnvironmentVariable("KeyVault:VaultUri")
             ?? "https://healthcare-vault.vault.azure.net/";
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        var secretVaultRef = await secretClient.GetSecretAsync("EventHubConnection");
        return new EventHubProducerClient(secretVaultRef.Value.Value, eventName);
    }
}
