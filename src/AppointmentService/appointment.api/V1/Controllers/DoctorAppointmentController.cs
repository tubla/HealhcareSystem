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
public class DoctorAppointmentController(IAppointmentService _appointmentService) : ControllerBase
{

    [HttpGet("doctor/{doctorId}/date/{date}")]
    public async Task<ActionResult<Response<IEnumerable<AppointmentResponseDto>>>> GetDoctorAppointmentsByDate(
        int doctorId,
        DateTime date,
        CancellationToken cancellationToken = default
    )
    {
        var userId = GetUserId();
        var response = await _appointmentService.GetDoctorAppointmentsByDateAsync(doctorId, date, userId, cancellationToken);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("check-doctor-appointment-exists")]
    public async Task<ActionResult<bool>> CheckDoctorAppointmentExists([FromBody] CheckDoctorAppointmentRequestDto dto, CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var response = await _appointmentService.GetDoctorAppointmentAsync(dto.DoctorId, userId, cancellationToken);
        return Ok(Response<bool>.Ok(response.Success));
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
    }
}
