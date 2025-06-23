using Microsoft.Extensions.Caching.Memory;
using shared.V1.HelperClasses.Contracts;

namespace shared.V1.HelperClasses.SecretClientHelper;

public class SecretProvider(IMemoryCache _cache) : ISecretProvider
{
    public string? GetSecret(string key)
    {
        return _cache.TryGetValue(key, out string? value) ? value : null;
    }
}
