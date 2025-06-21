using authentication.models.V1.Dtos;
using shared.V1.Models;

namespace authentication.services.V1.Contracts;

public interface IAuthService
{
    Task<Response<UserResponseDto>> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default);
    Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> CheckPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default);
    Task<bool> CheckUserExistsAsync(int userId, CancellationToken cancellationToken = default);
}
