namespace shared.V1.HelperClasses.Contracts;

public interface ISecretClient
{
    public Task<string> GetSecretValueAsync(string key);
}
