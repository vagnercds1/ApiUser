using ApiUser.Domain.Configurations;
using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ApiUser.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            BsonClassMap.RegisterClassMap<User>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            _users = database.GetCollection<User>("Users");
        }

        public Task CreateUserAsync(User user)
        {
            return _users.InsertOneAsync(user);
        }

        public async Task<List<User>> GetUserAsync(User user)
        {
            var filters = new List<FilterDefinition<User>>();
            var builder = Builders<User>.Filter;

            foreach (var property in typeof(User).GetProperties())
            {
                var value = property.GetValue(user);
                if (value != null && value.ToString() != string.Empty)
                {
                    filters.Add(builder.Eq(property.Name, value));
                }
            }

            var combinedFilter = filters.Count > 0 ? builder.And(filters) : builder.Empty;

            var users = await _users.Find(combinedFilter).ToListAsync();

            return users;
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            var cursor = await _users.FindAsync<User>(filter => filter.Id == id);
            return await cursor.FirstOrDefaultAsync();
        }

        public Task DeleteUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(int id, string value)
        {
            throw new NotImplementedException();
        }
    }
}
