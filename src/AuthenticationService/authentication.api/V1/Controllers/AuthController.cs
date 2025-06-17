using authentication.models.V1.Dtos;
using authentication.services.V1.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared.Models;

namespace authentication.api.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiVersion("1.0")]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<Response<UserDto>>> Register(
            [FromBody] RegisterRequestDto request,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);
            return response.Success
                ? CreatedAtAction(nameof(Register), new { id = response.Data!.UserId }, response)
                : BadRequest(response);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<Response<LoginResponseDto>>> Login(
            [FromBody] LoginRequestDto request,
            CancellationToken cancellationToken = default
        )
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        [HttpPost("check-permission")]
        [Authorize]
        public async Task<ActionResult<bool>> CheckPermission(
            [FromBody] PermissionCheckRequestDto request,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _authService.CheckPermissionAsync(
                request.UserId,
                request.PermissionName,
                cancellationToken
            );
            return Ok(result);
        }
    }
}
