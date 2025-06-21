using doctor.models.V1.Dto;
using doctor.services.V1.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared.V1.Models;
using System.Security.Claims;

namespace doctor.api.V1.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/doctors")]
[ApiVersion("1.0")]
[Authorize]
public class DoctorsController(IDoctorService _doctorService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Response<DoctorResponseDto>>> Create([FromBody] CreateDoctorRequestDto dto, CancellationToken cancellationToken = default)
    {
        int userId = GetUserId();
        var response = await _doctorService.CreateAsync(dto, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<DoctorResponseDto>>> GetById(int id, CancellationToken cancellationToken = default)
    {
        int userId = GetUserId();
        var response = await _doctorService.GetByIdAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    public async Task<ActionResult<Response<DoctorResponseDto>>> Update(int id, [FromBody] UpdateDoctorRequestDto dto, CancellationToken cancellationToken = default)
    {
        int userId = GetUserId();
        var response = await _doctorService.UpdateAsync(id, userId, dto, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Response<bool>>> Delete(int id, CancellationToken cancellationToken = default)
    {
        int userId = GetUserId();
        var response = await _doctorService.DeleteAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpPost("check-department-assigned")]
    public async Task<ActionResult<bool>> CheckDepartmentAsigned(
            [FromBody] DepartmentCheckRequestDto request,
            CancellationToken cancellationToken = default
        )
    {
        int userId = GetUserId();
        var result = await _doctorService.CheckDepartmentAsignedAsync(
            request.DeptId,
            userId,
            cancellationToken
        );
        return Ok(result);
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
    }
}
