using ApiUser.Domain.Configurations;
using ApiUser.Domain.Entities;
using ApiUser.Repository.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;

public class UserRepositoryTests
{
    private readonly Mock<IMongoCollection<User>> _mockCollection;
    private readonly Mock<IMongoDatabase> _mockDatabase;
    private readonly Mock<IMongoClient> _mockClient;
    private readonly Mock<IOptions<MongoDBSettings>> _mockSettings;
    private readonly Mock<ILogger<UserRepository>> _loggerMock;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _mockCollection = new Mock<IMongoCollection<User>>();
        _mockDatabase = new Mock<IMongoDatabase>();
        _mockClient = new Mock<IMongoClient>();
        _mockSettings = new Mock<IOptions<MongoDBSettings>>();
        _loggerMock = new Mock<ILogger<UserRepository>>();

        _mockSettings.Setup(s => s.Value).Returns(new MongoDBSettings
        {
            ConnectionString = "tese",
            DatabaseName = "TestDatabase"
        }); 
        _mockClient
            .Setup(c => c.GetDatabase(It.IsAny<string>(), null))
            .Returns(_mockDatabase.Object);
         
        _mockDatabase
            .Setup(d => d.GetCollection<User>(It.IsAny<string>(), null))
            .Returns(_mockCollection.Object);
         
        _userRepository = new UserRepository(_mockSettings.Object, _loggerMock.Object);

    }

    //[Fact]
    public async Task CreateUserAsync_ShouldInsertUser()
    {
        // Arrange
        var user = new User { Id = "1", Email = "test@example.com", Password = "password123" };

        // Configurar o mock para simular sucesso
        _mockCollection
            .Setup(c => c.InsertOneAsync(It.IsAny<User>(), null, default))
            .Returns(Task.CompletedTask);

        // Act
        var inserted = await _userRepository.CreateUserAsync(user);

        // Assert
        Assert.True(inserted);
        _mockCollection.Verify(c => c.InsertOneAsync(user, null, default), Times.Once);
    } 
}
