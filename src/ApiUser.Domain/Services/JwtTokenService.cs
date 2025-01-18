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

       return foundUser.Any() ? GenerateJwtToken(foundUser.First().Email): "";
    }

    private string GenerateJwtToken(string username)
    {
        string key = _configuration.GetSection("jwt:secretKey").Value!;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var symmetrickey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(symmetrickey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "yourdomain.com",
            audience: "yourdomain.com",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
