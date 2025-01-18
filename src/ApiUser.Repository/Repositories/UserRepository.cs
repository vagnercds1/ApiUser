using ApiUser.Domain.Configurations;
using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ApiUser.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;
    private readonly ILogger<UserRepository> _logger;
    public UserRepository(IOptions<MongoDBSettings> settings, ILogger<UserRepository> logger )
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);

        BsonClassMap.RegisterClassMap<User>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });

        _users = database.GetCollection<User>("Users");
        _logger = logger;
    }

    public async Task<bool> CreateUserAsync(User user)
    {
        try
        {
            await _users.InsertOneAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public async Task<List<User>> GetUserAsync(User user)
    {
        var filterBuilder = Builders<User>.Filter;
        var filter = filterBuilder.Empty;

        if (!string.IsNullOrEmpty(user.Email))
            filter &= filterBuilder.Eq(u => u.Email, user.Email);

        if (!string.IsNullOrEmpty(user.Password))
            filter &= filterBuilder.Eq(u => u.Password, user.Password);

        var users = await _users.Find(filter).ToListAsync();

        return users;
    }

    public async Task<User?> GetUserByIdAsync(string id)
    {
        var cursor = await _users.FindAsync<User>(filter => filter.Id == id);
        return await cursor.FirstOrDefaultAsync();
    }

    public async Task DeleteUserAsync(string id)
    {
        var idsFilter = Builders<User>.Filter.Eq(d => d.Id, id);

        await _users.FindOneAndDeleteAsync(filter: idsFilter);
    }

    public async Task UpdateUserAsync(User user)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
        var updateDefinition = Builders<User>.Update.Set(u => u, user);

        var resultado = await _users.UpdateOneAsync(filter, updateDefinition);
    }
}
