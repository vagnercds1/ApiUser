using ApiUser.Domain.Entities;
using ApiUser.Domain.Extentions;
using ApiUser.Domain.Interfaces;
using ApiUser.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiUser.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> PostUserAsync([FromBody] UserDto userDto)
        {
            try
            {
                var result = await _userService.CreateUserAsync(UserExtensions.ToEntityUser(userDto));

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

        [HttpGet("get/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //  [Authorize]
        public async Task<ActionResult<User>> GetUserAsync(string email)
        {
            try
            {
                var user = await _userService.GetUsersAsync(new User() {Email=email });

                return user.Any() ? 
                    Ok(user) : 
                    StatusCode(StatusCodes.Status404NotFound, "User not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //  [Authorize]
        public async Task<ActionResult<string>> Put(string id, [FromBody] UserDto userDto)
        {
            try
            { 
                var result = await _userService.UpdateUserAsync(id, userDto);

                if(result.StatusCode == System.Net.HttpStatusCode.OK)
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

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // [Authorize]
        public async Task<ActionResult<string>> Delete(string id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);

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


        //[HttpPost("login")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //public async Task<ActionResult<string>> Login(string user, string password)
        //{
        //    var token = await _userService.Authenticate(user, password);
        //    return Ok(token);
        //}
    }
}
