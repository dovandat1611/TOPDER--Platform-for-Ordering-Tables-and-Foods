using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Role;
using TOPDER.Service.Dtos.WalletTransaction;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/Roles
        [HttpGet("GetRoleList")]
        [SwaggerOperation(Summary = "Lấy danh sách roles: Admin")]
        public async Task<IActionResult> GetRoles([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _roleService.GetPagingAsync(pageNumber, pageSize);

            var response = new PaginatedResponseDto<RoleDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );

            return Ok(response);
        }

        // POST: api/Roles
        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo Role: Admin")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.AddAsync(roleDto);
            if (result)
            {
                return Ok("Tạo Role thành công");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new role.");
        }

        // GET: api/Roles/{id}
        [HttpGet("GetRole/{roleId}")]
        [SwaggerOperation(Summary = "Lấy role để update: Admin")]
        public async Task<IActionResult> GetRoleById(int roleId)
        {
            try
            {
                var roleDto = await _roleService.GetByIdAsync(roleId);
                return Ok(roleDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving role: {ex.Message}");
            }
        }

        // PUT: api/Roles/{id}
        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật role: Admin")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _roleService.UpdateAsync(roleDto);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
