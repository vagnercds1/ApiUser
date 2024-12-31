using ApiUser.Domain.Entities;
using ApiUser.Domain.Models;

namespace ApiUser.Domain.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> LoginUserAsync(LoginDto loginDto);
    }
}
