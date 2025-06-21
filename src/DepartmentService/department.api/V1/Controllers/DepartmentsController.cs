using department.models.V1.Dto;
using department.services.V1.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared.V1.Models;
using System.Security.Claims;

namespace department.api.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/departments")]
    [ApiVersion("1.0")]
    [Authorize]
    public class DepartmentsController(IDepartmentsService _departmentsService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<Response<DepartmentResponseDto>>> Create([FromBody] CreateDepartmentRequestDto dto, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var response = await _departmentsService.CreateAsync(dto, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<DepartmentResponseDto>>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var response = await _departmentsService.GetByIdAsync(id, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response<DepartmentResponseDto>>> Update(int id, [FromBody] UpdateDepartmentRequestDto dto, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var response = await _departmentsService.UpdateAsync(id, dto, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<bool>>> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            var response = await _departmentsService.DeleteAsync(id, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userIdClaim) ? 0 : int.Parse(userIdClaim);
        }
    }
}
