using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces.Repositories;
using ApiUser.Domain.Models;
using ApiUser.Domain.Services;
using Moq;
using System.Net;

namespace ApiUser.Domain.Test;

public class UseCaseUpdateUserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UseCaseUpdateUserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    } 
 
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
}
