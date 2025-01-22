using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces.Repositories;
using ApiUser.Domain.Models;
using ApiUser.Domain.Services;
using Moq;
using System.Net;

namespace ApiUser.Domain.Test;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    #region UseCase Create Users

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

    #endregion

    #region UserCase Read Users 

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

    #endregion

    #region UseCase Update Users

    [Fact]
    public async Task UpdateUserAsync_Exception_ReturnsError()
    {
        // Arrange
        var user = new Models.UserDto()
        {
            Email = "test@teste.com",
            FullName = "Vagner",
            Document = "123456789",
            Password = "123456"
        };

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User() { Id = "0000000", Email = "test@teste.com" });

        _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<User> { new User() { Id = "0000000", Email = "test@teste.com" } });


        _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()))
            .ThrowsAsync(new Exception("Error updating user"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _userService.UpdateUserAsync("0000000", user));

        Assert.Equal("Error updating user", exception.Message);
        _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(It.IsAny<string>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.GetUserAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_UserNotFound_ReturnsBadRequest()
    {
        // Act
        var result = await _userService.UpdateUserAsync("43423423", new UserDto());

        // Assert    
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("User not found", result.Message);
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), times: Times.Never);
    } 

    [Fact]
    public async Task UpdateUserAsync_UserEmailAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var user = new Models.UserDto()
        {
            Email = "test@teste.com",
            FullName = "Vagner",
            Document = "123456789",
            Password = "123456"
        };

        var listUser = new List<User>();
        listUser.Add(new User()
        {
            Id = "0000000",
            Email = "test@teste.com"
        });

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User() { Id = "xxxxxx", Email = "test@teste.com" });

        _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<User>()))
            .ReturnsAsync(listUser);

        _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()));

        // Act
        var result = await _userService.UpdateUserAsync("43423423", user);

        // Assert    
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("Email already registered.", result.Message);

        _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(It.IsAny<string>()), times: Times.Once);
        _userRepositoryMock.Verify(repo => repo.GetUserAsync(It.IsAny<User>()), times: Times.Once);
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), times: Times.Never);
    }

    [Fact]
    public async Task UpdateUserAsync_UpdatedUser_ReturnsOk()
    {
        // Arrange
        var user = new Models.UserDto()
        {
            Email = "test@teste.com",
            FullName = "Vagner",
            Document = "123456789",
            Password = "123456"
        };

        var listUser = new List<User>();
        listUser.Add(new User()
        {
            Id = "0000000",
            Email = "test@teste.com"
        });

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User() { Id = "0000000", Email = "test@teste.com" });

        _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<User>()))
            .ReturnsAsync(listUser);

        _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<User>()));

        // Act
        var result = await _userService.UpdateUserAsync("0000000", user);

        // Assert    
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal("User updated.", result.Message);

        _userRepositoryMock.Verify(repo => repo.GetUserByIdAsync(It.IsAny<string>()), times: Times.Once);
        _userRepositoryMock.Verify(repo => repo.GetUserAsync(It.IsAny<User>()), times: Times.Once);
        _userRepositoryMock.Verify(repo => repo.UpdateUserAsync(It.IsAny<User>()), times: Times.Once);
    }

    #endregion

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
