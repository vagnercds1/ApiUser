using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces;
using ApiUser.Domain.Interfaces.Repositories;
using ApiUser.Domain.Models;
using ApiUser.Domain.Validations;
using FluentValidation;
using FluentValidation.Results;
using System.Net;

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
        user.DateInsert = DateTime.UtcNow;
        UserValidationAdd validation = new(_repository);

        var validationResult = await validation.ValidateAsync(user);

        if (!validationResult.IsValid)
            return validationResult;

        await _repository.CreateUserAsync(user);

        return validationResult;
    }
    public async Task<List<User>> GetUsersAsync(User user)
    {
        var users = await _repository.GetUserAsync(user);

        return users;
    }

    public async Task<GenericValidationResult> UpdateUserAsync(string id, UserDto userDto)
    {
        var foundUserById = await _repository.GetUserByIdAsync(id: id);

        if (foundUserById == null)
            return new GenericValidationResult(statusCode: HttpStatusCode.BadRequest, "User not found");

        var foundUserByEmail = await _repository.GetUserAsync(new User() { Email = userDto.Email });

        if (foundUserByEmail
                 .Where(item => item.Email == userDto.Email && item.Id != foundUserById.Id)
                 .ToList().Any())
        {
            return new GenericValidationResult(statusCode: HttpStatusCode.BadRequest, "Email already registered.");
        }

        foundUserById.Email = userDto.Email;
        foundUserById.FullName = userDto.FullName;
        foundUserById.Password = userDto.Password;
        foundUserById.DateUpdate = DateTime.UtcNow;

        await _repository.UpdateUserAsync(foundUserById);

        return new GenericValidationResult(statusCode: HttpStatusCode.OK, "User updated.");
    }

    public async Task<GenericValidationResult> DeleteUserAsync(string id)
    {
        var foundUserById = await _repository.GetUserByIdAsync(id: id);

        if (foundUserById == null)
            return new GenericValidationResult(statusCode: HttpStatusCode.BadRequest, "User not found");

        // check data dependences...

        await _repository.DeleteUserAsync(id);

        return new GenericValidationResult(statusCode: HttpStatusCode.OK, "user permanently deleted.");
    }
}
