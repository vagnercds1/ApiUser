using ApiUserTest.Domain.Entities;

namespace ApiUserTest.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> IncludeUser(EntityUser user);

    Task<bool> UpdateUser(EntityUser user);

    Task<List<EntityUser>> GetUsers(EntityUser user);

    Task<bool> DeleteUser(EntityUser user);
}
