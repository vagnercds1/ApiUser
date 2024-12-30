using ApiUser.Domain.Models;
using ApiUser.Domain.Entities;
using FluentValidation.Results;

namespace ApiUser.Domain.Interfaces
{
    public interface IUserService
    {
        Task<ValidationResult> CreateUserAsync(User user);

        Task<List<User>> GetUsersAsync(User user);

        Task<ValidationResult> UpdateUserAsync(int id, UserDto userDto);

        Task<bool> DeleteUserAsync(User user);

        Task<User> LoginUserAsync(string user, string password);
    }
}
