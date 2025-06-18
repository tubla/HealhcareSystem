using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using patient.models.V1.Dto;
using patient.services.V1.Contracts;
using shared.Models;
using System.Security.Claims;

namespace patient.api.V1.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/patients")]
[ApiVersion("1.0")]
[Authorize]
public class PatientsController(IPatientService _patientService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Response<PatientDto>>> Create([FromBody] CreatePatientDto dto, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _patientService.CreateAsync(dto, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<PatientDto>>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _patientService.GetByIdAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Response<PatientDto>>> Update(int id, [FromBody] UpdatePatientDto dto, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _patientService.UpdateAsync(id, dto, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Response<bool>>> Delete(int id, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _patientService.DeleteAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}/appointments")]
    public async Task<ActionResult<Response<IEnumerable<AppointmentDto>>>> GetAppointments(int id, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _patientService.GetAppointmentsAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }
}
