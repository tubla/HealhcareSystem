using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prescription.models.V1.Dto;
using prescription.services.V1.Contracts;
using shared.Models;
using System.Security.Claims;

namespace prescription.api.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/prescriptions")]
    [ApiVersion("1.0")]
    [Authorize]
    public class PrescriptionsController(IPrescriptionService _prescriptionService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Response<PrescriptionDto>>> Create([FromBody] CreatePrescriptionDto dto, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _prescriptionService.CreateAsync(dto, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<PrescriptionDto>>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _prescriptionService.GetByIdAsync(id, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response<PrescriptionDto>>> Update(int id, [FromBody] UpdatePrescriptionDto dto, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _prescriptionService.UpdateAsync(id, dto, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<bool>>> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _prescriptionService.DeleteAsync(id, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpPost("{prescriptionId}/media")]
        public async Task<ActionResult<Response<MediaDto>>> UploadMedia(int prescriptionId, IFormFile file, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _prescriptionService.UploadMediaAsync(prescriptionId, file, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpGet("{prescriptionId}/media")]
        public async Task<ActionResult<Response<IEnumerable<MediaDto>>>> GetMedia(int prescriptionId, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _prescriptionService.GetMediaAsync(prescriptionId, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }
    }
}
