using AutoMapper;
using Humanizer;
using HybridApp.DTOs;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System.Data;


public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly RoleManager<Role> _roleManager;


    public RoleService(IRoleRepository roleRepository, IMapper mapper, RoleManager<Role> roleManager)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
        _roleManager = roleManager;

    }

    public async Task<RoleDto> GetRoleByIdAsync(string id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<IdentityResult> CreateRoleAsync(RoleDto roleDto)
    {
        // Check for duplicate name
        var existing = await _roleRepository.GetByNameAsync(roleDto.Name);
        if (existing != null)
        {
            Log.Warning("Duplicate role creation attempt: {RoleName}", roleDto.Name);
            return IdentityResult.Failed(new IdentityError { Description = "Role name already exists" });
        }


        // Map DTO → Entity
        var role = _mapper.Map<Role>(roleDto);

        // Ensure required fields
        role.Id = Guid.NewGuid().ToString();
        role.ConcurrencyStamp = Guid.NewGuid().ToString();
        role.NormalizedName = role.Name.ToUpper();
        role.Name = role.Name.Replace(" ", string.Empty);
        return await _roleManager.CreateAsync(role);


    }

    public async Task<IdentityResult> UpdateRoleAsync(RoleDto dto)
    {
        try
        {

            var role = await _roleRepository.GetByIdAsync(dto.Id);
            if (role == null)
                return IdentityResult.Failed(new IdentityError { Description = "Role not found" });

            // Check for duplicate name (excluding current role)
            var duplicate = await _roleRepository.GetByNameAsync(dto.Name);
            if (duplicate != null && duplicate.Id != dto.Id)
            {
                Log.Warning("Duplicate role update attempt: {RoleName}", dto.Name);
                return IdentityResult.Failed(new IdentityError { Description = "Role name already exists" });
            }


            // AutoMapper maps dto → role
            _mapper.Map(dto, role);

            return await _roleRepository.UpdateAsync(role);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unexpected error updating role {RoleId}", dto.Id);
            return IdentityResult.Failed(new IdentityError { Description = "Unexpected error occurred" });
        }

    }


    //public async Task DeleteRoleAsync(string id) =>
    //    await _roleRepository.DeleteAsync(id);
    public async Task<IdentityResult> DeleteRoleAsync(string id)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return IdentityResult.Failed(new IdentityError { Description = "Role not found" });

            await _roleRepository.DeleteAsync(id);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting role {RoleId}", id);
            return IdentityResult.Failed(new IdentityError { Description = "Unexpected error occurred" });
        }
    }

}