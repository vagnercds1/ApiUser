using ApiUser.Api.Controllers;
using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces;
using ApiUser.Domain.Models;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

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

    #region UseCase Create Users

    [Fact]
    public async Task PostUserAsync_ReturnsOk_WhenUserCreatedSuccessfully()
    {
        // Arrange
        var userDto = new UserDto { FullName = "Vagner", Email = "test@test.com" };
        _userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(new ValidationResult());

        // Act
        var result = await _controller.PostUserAsync(userDto);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("Created successfully", actionResult.Value);
    }

    [Fact]
    public async Task PostUserAsync_ReturnsException_NotCreated()
    { 
        // Arrange
        _userServiceMock
            .Setup(x => x.CreateUserAsync(It.IsAny<User>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.PostUserAsync(new UserDto());

        // Assert 
        Assert.True(((ObjectResult)result.Result!).StatusCode is (int)HttpStatusCode.InternalServerError);
        Assert.Equal(expected:"An unexpected error occurred. Please try again later.", 
                     actual: ((ObjectResult)result.Result!).Value!.ToString());
    }   

    [Fact]
    public async Task PostUserAsync_ReturnsMessage_NotCreatedUser()
    {
        // Arrange
        ValidationResult validationResult = new ValidationResult(); 
        validationResult.Errors.Add(new ValidationFailure("Email", "User Already Exists"));

        var userDto = new UserDto { FullName = "Vagner", Email = "test@test.com" };
        _userServiceMock.Setup(x => x.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(validationResult);

        // Act
        var result = await _controller.PostUserAsync(userDto);

        // Assert
        Assert.True(((ObjectResult)result.Result!).StatusCode is (int)HttpStatusCode.BadRequest);
        Assert.Equal(expected: "User Already Exists",
                     actual: ((ObjectResult)result.Result!).Value!.ToString());
    }

    #endregion
     
    #region UseCase Read Users

    [Fact]
    public async Task GetUserAsync_ReturnsException_NotReturnUsers()
    {
        // Arrange
        _userServiceMock
            .Setup(x => x.CreateUserAsync(It.IsAny<User>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.PostUserAsync(new UserDto());

        // Assert 
        Assert.True(((ObjectResult)result.Result!).StatusCode is (int)HttpStatusCode.InternalServerError);
        Assert.Equal(expected: "An unexpected error occurred. Please try again later.",
                     actual: ((ObjectResult)result.Result!).Value!.ToString());
    }     

    [Fact]
    public async Task GetUserAsync_ReturnsMessage_WhenUserNotExists()
    {
        // Arrange
        _userServiceMock.Setup(x => x.GetUsersAsync(It.IsAny<User>())).ReturnsAsync(new List<User>());

        // Act
        var result = await _controller.GetUserAsync("test@test.com");

        // Assert
        _userServiceMock.Verify(x => x.GetUsersAsync(It.IsAny<User>()), Times.Once);
        Assert.True(((ObjectResult)result.Result!).StatusCode is (int)HttpStatusCode.NotFound);
        Assert.Equal(expected: "User not found.",
                      actual: ((ObjectResult)result.Result!).Value!.ToString());
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

    #endregion

    #region UseCase Update Users

    // ...same logic above 

    #endregion

    #region UseCase Delete Users

    // ...same logic above 

    #endregion
}
