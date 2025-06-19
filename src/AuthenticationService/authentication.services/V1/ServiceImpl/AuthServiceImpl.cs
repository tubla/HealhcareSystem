using authentication.models.V1.Db;
using authentication.models.V1.Dtos;
using authentication.repositories.V1.Contracts;
using authentication.services.V1.Contracts;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace authentication.services.V1.ServiceImpl;

public class AuthServiceImpl(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IConfiguration _configuration) : IAuthService
{
    public async Task<Response<UserDto>> RegisterAsync(RegisterRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            return Response<UserDto>.Fail("Invalid registration data", 400);

        var existingUser = await _unitOfWork.UserRepository.GetByUsernameAsync(dto.Username, cancellationToken);
        if (existingUser != null)
            return Response<UserDto>.Fail("Username already exists", 409);

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Validate roles
        if (dto.RoleIds == null || !dto.RoleIds.Any())
            return Response<UserDto>.Fail("At least one role is required", 400);

        var validRoleIds = new List<int>();
        foreach (var roleId in dto.RoleIds.Distinct())
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Response<UserDto>.Fail($"Role ID {roleId} does not exist", 400);
            validRoleIds.Add(roleId);
        }

        try
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Save user to generate UserId
                await _unitOfWork.UserRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);

                // Add UserRole entries
                foreach (var roleId in validRoleIds)
                {
                    // Skip if UserRole already exists
                    var existingUserRole = await _unitOfWork.UserRepository.GetUserRoleAsync(user.UserId, roleId, cancellationToken);
                    if (existingUserRole != null)
                        continue;

                    var userRole = new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = roleId
                    };
                    await _unitOfWork.UserRepository.AddUserRoleAsync(userRole, cancellationToken);
                }
                await _unitOfWork.CompleteAsync(cancellationToken);

                return Response<UserDto>.Ok(_mapper.Map<UserDto>(user));
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            return Response<UserDto>.Fail($"Failed to register user: {ex.Message}", 500);
        }
    }

    public async Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return Response<LoginResponseDto>.Fail("Invalid login credentials", 400);

        var user = await _unitOfWork.UserRepository.GetByUsernameAsync(request.Username, cancellationToken);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Response<LoginResponseDto>.Fail("Invalid credentials", 401);

        var token = await GenerateJwtTokenAsync(user, cancellationToken);
        var response = new LoginResponseDto { Token = token, User = _mapper.Map<UserDto>(user) };

        return Response<LoginResponseDto>.Ok(response);
    }

    // This method will also be called from other services to check permissions
    public async Task<bool> CheckPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UserRepository.HasPermissionAsync(userId, permissionName, cancellationToken);
    }

    // This method will also be called from other services to check user existence
    public async Task<bool> CheckUserExistsAsync(int userId, CancellationToken cancellationToken = default)
    {
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
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
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
