using ApiUser.Domain.Interfaces;
using ApiUser.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiUser.Security.Controllers;

[Route("security/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase
{
    private readonly ILogger<AuthenticateController> _logger;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticateController(ILogger<AuthenticateController> logger, IJwtTokenService jwtTokenService)
    {
        _logger = logger;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// EndPoint used for authentication via previously registered email and password
    /// </summary>
    /// <param name="loginDto">Object containing email and password requested for authentication</param>
    /// <returns>
    /// returns an object containing the token
    //  
    //      "email": "test@test.com",
    //      "token": "85793408574935.55967850.69678"
    //  
    /// </returns>
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
                Ok(new {email = loginDto.Email, token = token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }
}
