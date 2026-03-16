using HybridApp.DTOs;
using Microsoft.AspNetCore.Identity;

public interface IRoleService
{
    Task<RoleDto> GetRoleByIdAsync(string id);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<IdentityResult> CreateRoleAsync(RoleDto roleDto);
    Task<IdentityResult> UpdateRoleAsync(RoleDto dto);

    Task<IdentityResult> DeleteRoleAsync(string id);
}

   