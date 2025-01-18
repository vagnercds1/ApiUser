using ApiUser.Api.Controllers;
using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces;
using ApiUser.Domain.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

public class UserControllerTests
{
    private readonly Mock<ILogger<UserController>> _loggerMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _loggerMock = new Mock<ILogger<UserController>>();
        _userServiceMock = new Mock<IUserService>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _controller = new UserController(_loggerMock.Object, _userServiceMock.Object, _jwtTokenServiceMock.Object);
    }

    [Fact]
    public async Task PostUserAsync_ReturnsOk_WhenUserCreatedSuccessfully()
    {
        // Arrange
        var userDto = new UserDto { FullName = "John Doe", Email = "john.doe@example.com" };
        _userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(new ValidationResult());

        // Act
        var result = await _controller.PostUserAsync(userDto);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("Created successfully", actionResult.Value);
    }
     
    [Fact]
    public async Task GetUserAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var email = "john.doe@example.com";
        var user = new User { FullName = "John Doe", Email = email };
        _userServiceMock.Setup(x => x.GetUsersAsync(It.IsAny<User>())).ReturnsAsync(new List<User> { user });

        // Act
        var result = await _controller.GetUserAsync(email);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(user, actionResult.Value);
    }

    [Fact]
    public async Task GetUserAsync_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _userServiceMock.Setup(x => x.GetUsersAsync(It.IsAny<User>())).ReturnsAsync(new List<User>());

        // Act
        var result = await _controller.GetUserAsync(email);

        // Assert
        var actionResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status404NotFound, actionResult.StatusCode);
        Assert.Equal("User not found.", actionResult.Value);
    }
}
