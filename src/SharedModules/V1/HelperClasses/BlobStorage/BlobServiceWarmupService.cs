using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.BlobStorage;

public class BlobServiceWarmupService(IConfiguration _configuration) : IHostedService, IBlobServiceClientProvider
{
    public BlobServiceClient Client { get; private set; } = null!;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var keyVaultUri = _configuration["KeyVault:VaultUri"]
            ?? Environment.GetEnvironmentVariable("KeyVault:VaultUri")
            ?? "https://healthcare-vault.vault.azure.net/";
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        var secretVaultRef = await secretClient.GetSecretAsync("BlobStorageConnection");
        Client = new BlobServiceClient(secretVaultRef.Value.Value);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
