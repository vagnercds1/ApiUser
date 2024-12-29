using ApiUserTest.Domain.Entities;
using ApiUserTest.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiUserTest.Infrastruture.Repositories
{
    internal class UserRepository : IUserRepository
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

        public Task<bool> UpdateUser(EntityUser user)
        {
            throw new NotImplementedException();
        }
    }
}
