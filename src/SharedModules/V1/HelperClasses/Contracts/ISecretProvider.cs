namespace shared.V1.HelperClasses.Contracts;

public interface ISecretProvider
{
    string? GetSecret(string key);
}
