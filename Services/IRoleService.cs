public interface IRoleService
{
    Task<RoleDto> GetRoleByIdAsync(string id);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task CreateRoleAsync(RoleDto roleDto);
    Task UpdateRoleAsync(RoleDto roleDto);
    Task DeleteRoleAsync(string id);
}