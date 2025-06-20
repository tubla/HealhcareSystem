using appointment.models.V1.Dtos;
using appointment.services.V1.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared.Models;
using System.Security.Claims;

namespace appointment.api.V1.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/appointments")]
[ApiVersion("1.0")]
[Authorize]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<ActionResult<Response<AppointmentDto>>> CreateAppointment(
        [FromBody] CreateAppointmentDto dto
    )
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _appointmentService.CreateAppointmentAsync(dto, userId);
        return response.Success
            ? CreatedAtAction(
                nameof(GetAppointment),
                new { id = response.Data!.AppointmentId },
                response
            )
            : BadRequest(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<AppointmentDto>>> GetAppointment(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _appointmentService.GetAppointmentAsync(id, userId);
        return response.Success ? Ok(response) : NotFound(response);
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<ActionResult<Response<IEnumerable<AppointmentDto>>>> GetDoctorAppointments(
        int doctorId
    )
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _appointmentService.GetDoctorAppointmentsAsync(doctorId, userId);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<Response<AppointmentDto>>> CancelAppointment(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _appointmentService.CancelAppointmentAsync(id, userId);
        return response.Success ? Ok(response) : BadRequest(response);
    }

    [HttpPost("check-appointment-exists")]
    public async Task<ActionResult<bool>> CheckAppointmentExists(CheckAppointmentDto dto, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _appointmentService.GetAppointmentAsync(dto.AppointmentId, userId);
        return response.Success ? Ok(true) : Ok(false);
    }
}
