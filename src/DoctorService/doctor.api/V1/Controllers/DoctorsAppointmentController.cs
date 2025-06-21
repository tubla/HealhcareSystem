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
public class DoctorsAppointmentController(IDoctorService _doctorService) : ControllerBase
{
    [HttpGet("{id}/appointments/date/{date}")]
    public async Task<ActionResult<Response<IEnumerable<AppointmentResponseDto>>>> GetAppointments(int id, DateTime date, CancellationToken cancellationToken = default)
    {
        int userId = GetUserId();
        var response = await _doctorService.GetAppointmentsAsync(id, date, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
    }
}
