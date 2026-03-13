
using HybridApp.DTOs;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(string id);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task CreateUserAsync(UserDto userDto);
    Task UpdateUserAsync(UserDto userDto);
    Task DeleteUserAsync(string id);
}