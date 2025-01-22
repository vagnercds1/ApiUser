using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces.Repositories;
using ApiUser.Domain.Models;
using ApiUser.Domain.Services;
using Moq;
using System.Net;

namespace ApiUser.Domain.Test;

public class UseCaseCreateUserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UseCaseCreateUserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    } 
  
    [Fact]
    public async Task CreateUserAsync_ExceptionOnCreate_ReturnsMessage()
    {
        // Arrange

        var listUser = new List<User> { new User { Email = "test@teste.com" } };

        object value = _userRepositoryMock.Setup(repo => repo.CreateUserAsync(new User()))
            .ThrowsAsync(new Exception("Repository failure"));


        _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<User>()))
            .ReturnsAsync(listUser);

        // Act
        var result = await _userService.CreateUserAsync(new User());

        // Assert
        Assert.False(result.IsValid);
        _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_UserAlreadyExists_ReturnsMessage()
    {
        // Arrange

        var listUser = new List<User> { new User { Email = "test@teste.com" } };

        object value = _userRepositoryMock.Setup(repo => repo.CreateUserAsync(new User()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<User>()))
            .ReturnsAsync(listUser);

        // Act
        var result = await _userService.CreateUserAsync(new User());

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Where(a => a.ErrorMessage == "User Already Exists").Any());
        _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_UserCreated_ReturnsOK()
    {
        // Arrange
        var user = new User()
        {
            Email = "test@teste.com",
            FullName = "Vagner",
            Document = "123456789",
            Password = "123456"
        };

        var listUser = new List<User>();

        object value = _userRepositoryMock.Setup(repo => repo.CreateUserAsync(new User()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<User>()))
            .ReturnsAsync(listUser);

        // Act
        var result = await _userService.CreateUserAsync(user);

        // Assert
        Assert.True(result.IsValid);
        _userRepositoryMock.Verify(repo => repo.CreateUserAsync(It.IsAny<User>()), times: Times.Once);
    } 
}
