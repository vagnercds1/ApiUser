﻿using ApiUser.Domain.Entities;
using ApiUser.Domain.Models;
using FluentValidation.Results;

namespace ApiUser.Domain.Interfaces;

public interface IUserService
{
    Task<ValidationResult> CreateUserAsync(User user);

    Task<List<User>> GetUsersAsync(User user);

    Task<GenericValidationResult> UpdateUserAsync(string id, UserDto userDto);

    Task<GenericValidationResult> DeleteUserAsync(string id);         
}
