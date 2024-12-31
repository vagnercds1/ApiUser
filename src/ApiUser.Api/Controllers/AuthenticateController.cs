using ApiUser.Domain.Interfaces;
using ApiUser.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiUser.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ILogger<AuthenticateController> _logger; 
        private readonly IJwtTokenService _jwtTokenService;

        public AuthenticateController(ILogger<AuthenticateController> logger,  IJwtTokenService jwtTokenService)
        {
            _logger = logger; 
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto)
        { 
            try
            {
                string token = await _jwtTokenService.LoginUserAsync(loginDto);

                return String.IsNullOrEmpty(token) ?
                    StatusCode(StatusCodes.Status401Unauthorized, "User or Password incorrect.") :
                    Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}
