using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using authentication.models.V1.Db;
using authentication.models.V1.Dtos;
using authentication.repositories.V1.Contracts;
using authentication.services.V1.Contracts;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using shared.Models;

namespace authentication.services.V1.ServiceImpl;

public class AuthServiceImpl(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IConfiguration _configuration
) : IAuthService
{
    public async Task<Response<UserDto>> RegisterAsync(UserDto dto, string password)
    {
        var existingUser = await _unitOfWork.Users.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
            return Response<UserDto>.Fail("Username already exists");

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return Response<UserDto>.Ok(_mapper.Map<UserDto>(user));
    }

    public async Task<Response<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Response<LoginResponseDto>.Fail("Invalid credentials");

        var token = GenerateJwtToken(user);
        var response = new LoginResponseDto { Token = token, User = _mapper.Map<UserDto>(user) };

        return Response<LoginResponseDto>.Ok(response);
    }

    public async Task<bool> CheckPermissionAsync(int userId, string permissionName)
    {
        var hasPermission = await _unitOfWork.Users.HasPermissionAsync(userId, permissionName);
        if (!hasPermission)
            throw new services.V1.CustomExceptions.UnauthorizedAccessException(
                $"User {userId} lacks permission {permissionName}"
            );

        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.RoleName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
