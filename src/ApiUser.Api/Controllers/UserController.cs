﻿using ApiUser.Domain.Entities;
using ApiUser.Domain.Extentions;
using ApiUser.Domain.Interfaces;
using ApiUser.Domain.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiUser.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;

        public UserController(ILogger<UserController> logger, IUserService userService, IJwtTokenService jwtTokenService)
        {
            _logger = logger;
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// EndPoint used for create new users
        /// </summary>
        /// <param name="userDto">Object containing users parameter</param>
        /// <returns>Return information messages about user creation </returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> PostUserAsync([FromBody] UserDto userDto)
        {
            try
            {
                ValidationResult result = await _userService.CreateUserAsync(UserExtensions.ToEntityUser(userDto));

                if (!result.IsValid)
                    return BadRequest(String.Join(", ", result.Errors.Select(x => x.ErrorMessage).ToList()));

                return Ok("Created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Return user information based on email
        /// </summary>
        /// <param name="email">Parameter used for search the user</param>
        /// <returns>Return objet user</returns>
        [HttpGet("get/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<User>> GetUserAsync(string email)
        {
            try
            {
                var user = await _userService.GetUsersAsync(new User() { Email = email });

                return user.Any() ?
                    Ok(user.First()) :
                    StatusCode(StatusCodes.Status404NotFound, "User not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="userDto">Object with the parameters that can be updated</param>
        /// <returns>Return information messages about user update</returns>
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<string>> Put(string id, [FromBody] UserDto userDto)
        {
            try
            {
                GenericValidationResult result = await _userService.UpdateUserAsync(id, userDto);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    return Ok(result.Message);
                else
                    return StatusCode(Convert.ToInt32(result.StatusCode), result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Delete user by Id
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Return information messages about user update</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<ActionResult<string>> Delete(string id)
        {
            try
            {
                GenericValidationResult result = await _userService.DeleteUserAsync(id);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    return Ok(result.Message);
                else
                    return StatusCode(Convert.ToInt32(result.StatusCode), result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}
