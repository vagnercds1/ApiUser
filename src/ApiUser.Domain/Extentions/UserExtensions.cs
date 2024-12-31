using ApiUser.Domain.Entities;
using ApiUser.Domain.Models;

namespace ApiUser.Domain.Extentions;

public static class UserExtensions
{
    public static User ToEntityUser(this UserDto requestUser)
    {
        if (requestUser == null)
            throw new ArgumentNullException(nameof(requestUser));

        return new User
        {
            FullName = requestUser.FullName,
            Document = requestUser.Document,
            Email = requestUser.Email,
            Password = requestUser.Password,
            Role = requestUser.Role
        };
    }
}
