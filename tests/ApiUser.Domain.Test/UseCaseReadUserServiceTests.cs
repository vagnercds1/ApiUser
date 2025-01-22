using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces.Repositories;
using ApiUser.Domain.Models;
using ApiUser.Domain.Services;
using Moq;
using System.Net;

namespace ApiUser.Domain.Test;

public class UseCaseReadUserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UseCaseReadUserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    } 
 
    [Fact]
    public async Task GetUsersAsync_ThrowsException_WhenRepositoryFails()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };

        _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<User>()))
            .ThrowsAsync(new Exception("Error fetching users"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _userService.GetUsersAsync(user));

       
        Assert.Equal("Error fetching users", exception.Message);       
        _userRepositoryMock.Verify(repo => repo.GetUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task GetUsersAsync_HasUsers_ReturnsUserList()
    {
        // Arrange
        var user = new User { Email = "test@example.com" };
        var users = new List<User> { new User { Email = "test@example.com" } };
        _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<User>())).ReturnsAsync(users);

        // Act
        var result = await _userService.GetUsersAsync(user);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("test@example.com", result.First().Email);
    }

    
}
