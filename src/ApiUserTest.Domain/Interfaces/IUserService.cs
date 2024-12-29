using ApiUserTest.Domain.Entities;

namespace ApiUserTest.Domain.Interfaces
{
    public interface IUserService
    {
        Task<bool> IncludeUser(EntityUser user);

        Task<List<EntityUser>> GetUsers(EntityUser user);

        Task<bool> UpdateUser(EntityUser user);

        Task<bool> DeleteUser(EntityUser user);

        Task<EntityUser> LoginUser(string user, string password);
    }
}
