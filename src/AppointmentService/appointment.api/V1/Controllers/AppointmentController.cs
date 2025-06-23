using appointment.models.V1.Dtos;
using appointment.services.V1.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared.V1.Models;
using System.Security.Claims;

namespace appointment.api.V1.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/appointments")]
[ApiVersion("1.0")]
[Authorize]
public class AppointmentController(IAppointmentService _appointmentService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Response<AppointmentResponseDto>>> CreateAppointment(
        [FromBody] CreateAppointmentRequestDto dto,
        CancellationToken cancellationToken = default
    )
    {
        var userId = GetUserId();
        var response = await _appointmentService.CreateAppointmentAsync(dto, userId, cancellationToken);
        return response.Success
            ? CreatedAtAction(
                nameof(GetAppointment),
                new { id = response.Data!.AppointmentId },
                response
            )
            : BadRequest(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<AppointmentResponseDto>>> GetAppointment(int id, CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var response = await _appointmentService.GetAppointmentAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<Response<AppointmentResponseDto>>> CancelAppointment(int id, CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var response = await _appointmentService.CancelAppointmentAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Response<AppointmentResponseDto>>> UpdateAppointment(
    int id,
    [FromBody] UpdateAppointmentRequestDto dto,
    CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var response = await _appointmentService.UpdateAppointmentAsync(id, userId, dto, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Response<bool>>> DeleteAppointment(
    int id,
    CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var response = await _appointmentService.DeleteAppointmentAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);

    }

    [HttpPost("check-appointment-exists")]
    public async Task<ActionResult<bool>> CheckAppointmentExists([FromBody] CheckAppointmentRequestDto dto, CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var response = await _appointmentService.CheckAppointmentExistsAsync(dto.AppointmentId, userId, cancellationToken);
        return Ok(response);
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
    }
}
