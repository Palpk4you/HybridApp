using AutoMapper;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
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
        var role = _mapper.Map<Role>(roleDto);
        await _roleRepository.AddAsync(role);
    }

    public async Task UpdateRoleAsync(RoleDto roleDto)
    {
        var role = _mapper.Map<Role>(roleDto);
        await _roleRepository.UpdateAsync(role);
    }

    public async Task DeleteRoleAsync(string id) =>
        await _roleRepository.DeleteAsync(id);
}