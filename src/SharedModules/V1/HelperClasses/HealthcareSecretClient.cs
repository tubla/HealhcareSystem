//using Azure.Identity;
//using Azure.Security.KeyVault.Secrets;
//using Microsoft.Extensions.Configuration;
//using shared.V1.HelperClasses.Contracts;
//using System.Collections.Concurrent;

//namespace shared.V1.HelperClasses;

//internal class HealthcareSecretClient(IConfiguration _configuration) : ISecretClient
//{
//    private static readonly ConcurrentDictionary<string, (string Value, DateTime Expiry)> _secretCache = new();
//    private static readonly TimeSpan _cacheDuration = TimeSpan.FromDays(2);

//    public async Task<string> GetSecretValueAsync(string key)
//    {
//        var now = DateTime.UtcNow;
//        if (_secretCache.TryGetValue(key, out var entry) && entry.Expiry > now)
//            return entry.Value;

//        var keyVaultUri = _configuration["KeyVault:VaultUri"]
//            ?? Environment.GetEnvironmentVariable("KeyVault:VaultUri")
//            ?? "https://healthcare-vault.vault.azure.net/";
//        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
//        var secretValue = (await secretClient.GetSecretAsync(key)).Value.Value;

//        _secretCache[key] = (secretValue, now.Add(_cacheDuration));
//        return secretValue;
//    }
//}