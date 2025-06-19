namespace shared.HelperClasses.Contracts;

public interface IAuthServiceProxy
{
    Task<bool> CheckPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default);
    Task<bool> CheckUserExistsAsync(int userId, CancellationToken cancellationToken = default);
}
