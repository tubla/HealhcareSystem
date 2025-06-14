using authentication.models.V1.Dtos;
using shared.Models;

namespace authentication.services.V1.Contracts;

public interface IAuthService
{
    Task<Response<UserDto>> RegisterAsync(UserDto dto, string password);
    Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto request);
    Task<bool> CheckPermissionAsync(int userId, string permissionName);
}
