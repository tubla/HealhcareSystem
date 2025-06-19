using department.models.V1.Dto;
using department.services.V1.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shared.Models;
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
        public async Task<ActionResult<Response<DepartmentDto>>> Create([FromBody] CreateDepartmentDto dto, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _departmentsService.CreateAsync(dto, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response<DepartmentDto>>> GetById(int id, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _departmentsService.GetByIdAsync(id, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response<DepartmentDto>>> Update(int id, [FromBody] UpdateDepartmentDto dto, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _departmentsService.UpdateAsync(id, dto, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response<bool>>> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var response = await _departmentsService.DeleteAsync(id, userId, cancellationToken);
            return response.Success ? Ok(response) : StatusCode(response.StatusCode, response);
        }
    }
}
