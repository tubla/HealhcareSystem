using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using patient.models.V1.Dto;
using patient.services.V1.Contracts;
using shared.V1.Models;
using System.Security.Claims;

namespace patient.api.V1.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/patients")]
[ApiVersion("1.0")]
[Authorize]
public class PatientsController(IPatientService _patientService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Response<PatientResponseDto>>> Create([FromBody] CreatePatientRequestDto dto, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _patientService.CreateAsync(dto, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Response<PatientResponseDto>>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _patientService.GetByIdAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Response<PatientResponseDto>>> Update(int id, [FromBody] UpdatePatientRequestDto dto, CancellationToken cancellationToken = default)
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
    public async Task<ActionResult<Response<IEnumerable<AppointmentResponseDto>>>> GetAppointments(int id, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _patientService.GetAppointmentsAsync(id, userId, cancellationToken);
        return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
    }

    [HttpPost("check-patient-exists")]
    public async Task<ActionResult<bool>> CheckPatientExists(CheckPatientExistenceRequestDto dto, CancellationToken cancellationToken = default)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var response = await _patientService.CheckPatientExistsAsync(dto.PatientId, userId, cancellationToken);
        return Ok(response);
    }
}
