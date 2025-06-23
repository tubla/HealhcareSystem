using Azure.Storage.Blobs;
using Microsoft.Extensions.Hosting;
using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.BlobStorage;

public class BlobServiceWarmupService(ISecretProvider _secretProvider) : IHostedService, IBlobServiceClientProvider
{
    public BlobServiceClient Client { get; private set; } = null!;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var connection = _secretProvider.GetSecret("BlobStorageConnection");
        Client = new BlobServiceClient(connection);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
