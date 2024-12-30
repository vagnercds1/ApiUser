using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces;
using ApiUser.Domain.Interfaces.Repositories;
using ApiUser.Domain.Models;
using ApiUser.Domain.Validations;
using FluentValidation.Results;

namespace ApiUser.Domain.Services;

public class UserService : IUserService
{

    private readonly IUserRepository _repository;

    public UserService(IUserRepository userRepository)
    {
        _repository = userRepository;
    }

    public async Task<ValidationResult> CreateUserAsync(User user)
    {
        UserValidationAdd validation = new(_repository);         

        var validationResult = await validation.ValidateAsync(user);

        if (!validationResult.IsValid)
            return validationResult;

        await _repository.CreateUserAsync(user);

        return validationResult;
    }

    public async Task<ValidationResult> UpdateUserAsync(int id, UserDto userDto)
    {
       


    }

    public async Task<List<User>> GetUsersAsync(User user)
    { 
        var users = await _repository.GetUserAsync(user);

        return users;
    }

    public Task<bool> DeleteUserAsync(User user)
    {
        throw new NotImplementedException();
    }



    public Task<User> LoginUserAsync(string user, string password)
    {
        throw new NotImplementedException();
    }

  
}
