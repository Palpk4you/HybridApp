using HybridApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridApp.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RolesApiController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesApiController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return role == null ? NotFound() : Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleDto dto)
        {
            await _roleService.CreateRoleAsync(dto);
            return Ok("Role created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] RoleDto dto)
        {
            dto.Id = id;
            await _roleService.UpdateRoleAsync(dto);
            return Ok("Role updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _roleService.DeleteRoleAsync(id);
            return Ok("Role deleted successfully");
        }
    }
}