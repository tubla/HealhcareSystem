using authentication.models.V1.Db;
using authentication.models.V1.Dtos;
using authentication.repositories.V1.Contracts;
using authentication.services.V1.Contracts;
using authentication.services.V1.CustomExceptions;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using shared.V1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace authentication.services.V1.ServiceImpl;

public class AuthServiceImpl(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IConfiguration _configuration) : IAuthService
{

    private async Task ValidateRegistrationInputAsync(RegisterRequestDto dto, CancellationToken cancellationToken)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Invalid registration data");

        if (string.IsNullOrWhiteSpace(dto.Username))
            throw new ArgumentException("Username is required", nameof(dto.Username));

        if (string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("Password is required", nameof(dto.Password));

        if (await _unitOfWork.UserRepository.GetByUsernameAsync(dto.Username, cancellationToken) != null)
            throw new InvalidOperationException("Username already exists");

        if (dto.RoleIds == null || !dto.RoleIds.Any())
            throw new ArgumentException("At least one role is required", nameof(dto.RoleIds));

        foreach (var roleId in dto.RoleIds.Distinct())
        {
            if (await _unitOfWork.RoleRepository.GetByIdAsync(roleId, cancellationToken) == null)
                throw new RecordNotFoundException($"Role ID {roleId} does not exist");
        }
    }

    public async Task<Response<UserResponseDto>> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateRegistrationInputAsync(dto, cancellationToken);

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        return await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            foreach (var roleId in dto.RoleIds!.Distinct())
            {
                if (await _unitOfWork.UserRepository.GetUserRoleAsync(user.UserId, roleId, cancellationToken) != null)
                    continue;

                var userRole = new UserRole { UserId = user.UserId, RoleId = roleId };
                await _unitOfWork.UserRepository.AddUserRoleAsync(userRole, cancellationToken);
            }
            await _unitOfWork.CompleteAsync(cancellationToken);

            return Response<UserResponseDto>.Ok(_mapper.Map<UserResponseDto>(user));
        }, cancellationToken);
    }

    private async Task<User> ValidateLoginCredentialsAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "Invalid login credentials");

        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Invalid username", nameof(request.Username));

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new ArgumentException("Invalid password", nameof(request.Password));

        var user = await _unitOfWork.UserRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new CustomExceptions.UnauthorizedAccessException("Invalid credentials");

        return user;
    }

    public async Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await ValidateLoginCredentialsAsync(request, cancellationToken);
        var token = await GenerateJwtTokenAsync(user, cancellationToken);
        var response = new LoginResponseDto { Token = token, User = _mapper.Map<UserResponseDto>(user) };
        return Response<LoginResponseDto>.Ok(response);
    }

    // This method will also be called from other services to check permissions
    public async Task<bool> CheckPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(permissionName))
            return false;

        return await _unitOfWork.UserRepository.HasPermissionAsync(userId, permissionName, cancellationToken);
    }

    // This method will also be called from other services to check user existence
    public async Task<bool> CheckUserExistsAsync(int userId, CancellationToken cancellationToken = default)
    {
        if (userId <= 0)
            return false;

        return await _unitOfWork.UserRepository.GetByIdAsync(userId, cancellationToken) != null;
    }

    private async Task<string> GenerateJwtTokenAsync(User user, CancellationToken cancellationToken = default)
    {
        var userWithRoles = await _unitOfWork.UserRepository.GetByIdWithRolesAsync(user.UserId, cancellationToken);
        var roleClaims = userWithRoles!.UserRoles
            .Select(ur => new Claim(ClaimTypes.Role, ur.Role.RoleName))
            .ToArray();

        var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.UserName)
            };
        claims.AddRange(roleClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
