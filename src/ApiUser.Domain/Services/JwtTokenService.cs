using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces;
using ApiUser.Domain.Interfaces.Repositories;
using ApiUser.Domain.Models;
using ApiUser.Domain.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiUser.Domain.Services;

public class JwtTokenService: IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _repository; 
    public JwtTokenService(IConfiguration configuration, IUserRepository userRepository)
    {
        _configuration = configuration;
        _repository = userRepository; 
    }

    public async Task<string> LoginUserAsync(LoginDto loginDto)
    {
        UserValidationLogin validation = new(_repository);

        var validationResult = await validation.ValidateAsync(loginDto);

        if (!validationResult.IsValid)
            return String.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        var foundUser = await _repository.GetUserAsync(
            new User()
            {
                Email = loginDto.Email,
                Password = loginDto.Password
            });

        if (foundUser.Any())
            return GenerateToken(foundUser.First());
        else
            return "";
    }

    private string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_configuration.GetSection("jwt:secretKey").ToString()!);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(user),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(2),
        };

        var token = handler.CreateToken(tokenDescriptor);

        var strToken = handler.WriteToken(token);

        return strToken;
    }

    private ClaimsIdentity GenerateClaims(User user)
    {
        var ci = new ClaimsIdentity();

        ci.AddClaim(claim: new Claim(type: ClaimTypes.Name, value: user.Email));

        foreach (string role in user.Roles)
            ci.AddClaim(claim: new Claim(type: ClaimTypes.Role, value: role));

        return ci;
    }
}
