using doctor.models.V1.Dto;
using doctor.services.V1.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared.Models;
using System.Security.Claims;

namespace doctor.api.V1.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/doctors")]
[ApiVersion("1.0")]
[Authorize]
public class DoctorsController(IDoctorService _doctorService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Response<DoctorDto>>> Create([FromBody] CreateDoctorDto dto, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _doctorService.CreateAsync(dto, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<DoctorDto>>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _doctorService.GetByIdAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    public async Task<ActionResult<Response<DoctorDto>>> Update(int id, [FromBody] UpdateDoctorDto dto, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _doctorService.UpdateAsync(id, userId, dto, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Response<bool>>> Delete(int id, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _doctorService.DeleteAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<Response<IEnumerable<AppointmentDto>>>> GetAppointments(int id, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _doctorService.GetAppointmentsAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }
}
