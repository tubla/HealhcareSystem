using Azure.Storage.Blobs;

namespace shared.V1.HelperClasses.Contracts;

public interface IBlobServiceClientProvider
{
    BlobServiceClient Client { get; }
}
