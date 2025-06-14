namespace appointment.services.V1.Contracts;

public interface IAuthServiceProxy
{
    Task<bool> CheckPermissionAsync(int userId, string permissionName);
}
