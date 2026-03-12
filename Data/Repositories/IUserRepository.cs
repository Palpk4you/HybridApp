public interface IUserRepository
{
    Task<User?> GetByIdAsync(string id);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(string id);
}