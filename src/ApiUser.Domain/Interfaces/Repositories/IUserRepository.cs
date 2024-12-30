using ApiUser.Domain.Entities;

namespace ApiUser.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task CreateUserAsync(User user); 
        Task<List<User>> GetUserAsync(User user);
        Task<User?> GetUserByIdAsync(string id);
        Task UpdateUserAsync(int id, string value);
        Task DeleteUserAsync(int id); 
    }
}
