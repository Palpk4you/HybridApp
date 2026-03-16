using Microsoft.AspNetCore.Identity;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(string id);
    Task<IEnumerable<Role>> GetAllAsync();
    Task AddAsync(Role role);
    Task<IdentityResult> UpdateAsync(Role role);

    Task DeleteAsync(string id);
    
}