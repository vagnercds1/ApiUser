using ApiUser.Domain.Entities;

namespace ApiUser.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<bool> CreateUserAsync(User user);
        Task<List<User>> GetUserAsync(User user);
        Task<User?> GetUserByIdAsync(string id);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string id);
    }
}
