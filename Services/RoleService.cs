using AutoMapper;
using HybridApp.DTOs;
using Microsoft.AspNetCore.Identity;
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

    public async Task CreateRoleAsync(RoleDto roleDto)
    {
        // Map DTO → Entity
        var role = _mapper.Map<Role>(roleDto);

        // Ensure required fields
        role.Id = Guid.NewGuid().ToString();
        role.ConcurrencyStamp = Guid.NewGuid().ToString();
        role.NormalizedName = role.Name.ToUpper();
        role.Name = role.Name.Replace(" ", string.Empty);
        await _roleManager.CreateAsync(role);


    }

    public async Task<IdentityResult> UpdateRoleAsync(RoleDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(dto.Id);
        if (role == null)
            return IdentityResult.Failed(new IdentityError { Description = "Role not found" });

        // AutoMapper maps dto → role
        _mapper.Map(dto, role);

        return await _roleRepository.UpdateAsync(role);

    }


    public async Task DeleteRoleAsync(string id) =>
        await _roleRepository.DeleteAsync(id);
}