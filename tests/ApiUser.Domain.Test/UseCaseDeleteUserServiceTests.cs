using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces.Repositories;
using ApiUser.Domain.Models;
using ApiUser.Domain.Services;
using Moq;
using System.Net;

namespace ApiUser.Domain.Test;

public class UseCaseDeleteUserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UseCaseDeleteUserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }
     
    #region UseCase Delete Users

    [Fact]
    public async Task DeleteUserAsync_UserNotFound_ReturnsBadRequest()
    {
        // Arrange
        var userId = "1";
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("User not found", result.Message);
    }

    [Fact]
    public async Task DeleteUserAsync_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var userId = "1";
        var user = new User { Id = userId };
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _userRepositoryMock.Setup(repo => repo.DeleteUserAsync(userId)).Returns(Task.CompletedTask);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("user permanently deleted.", result.Message);
        _userRepositoryMock.Verify(repo => repo.DeleteUserAsync(userId), Times.Once);
    }

    #endregion
}
