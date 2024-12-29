using ApiUserTest.Domain.Entities;
using ApiUserTest.Domain.Interfaces;

namespace ApiUserTest.Domain.Services
{
    public class UserService : IUserService
    {
        public Task<bool> DeleteUser(EntityUser user)
        {
            throw new NotImplementedException();
        }

        public Task<List<EntityUser>> GetUsers(EntityUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IncludeUser(EntityUser user)
        {
            throw new NotImplementedException();
        }

        public Task<EntityUser> LoginUser(string user, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUser(EntityUser user)
        {
            throw new NotImplementedException();
        }
    }
}
