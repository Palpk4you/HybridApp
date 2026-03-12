using AutoMapper;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto> GetUserByIdAsync(string id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task CreateUserAsync(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        await _userRepository.AddAsync(user);
    }

    public async Task UpdateUserAsync(UserDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(string id) =>
        await _userRepository.DeleteAsync(id);
}