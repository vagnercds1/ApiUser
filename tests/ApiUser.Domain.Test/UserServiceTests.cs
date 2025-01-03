﻿using ApiUser.Domain.Entities;
using ApiUser.Domain.Interfaces.Repositories;
using ApiUser.Domain.Models;
using ApiUser.Domain.Services;
using Moq;
using System.Net;

namespace ApiUser.Domain.Test
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        //[Fact]
        //public async Task CreateUserAsync_ValidUser_ReturnsValidationResultSuccess()
        //{
        //    // Arrange
        //    var user = new User { Email = "test@example.com", FullName = "Test User" };
        //    _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>())).Returns(true);
        //    _userRepositoryMock.Setup(repo => repo.GetUserAsync(It.IsAny<User>())).ReturnsAsync(new List<User>());

        //    // Act
        //    var result = await _userService.CreateUserAsync(user);

        //    // Assert
        //    Assert.False(result.IsValid); 
        //}

        [Fact]
        public async Task GetUsersAsync_ValidUser_ReturnsUserList()
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

        //[Fact]
        //public async Task UpdateUserAsync_UserNotFound_ReturnsBadRequest()
        //{
        //    // Arrange
        //    var userId = "1";
        //    var userDto = new UserDto { Email = "new@example.com", FullName = "New User", Password = "newpass" };
        //    _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

        //    // Act
        //    var result = await _userService.UpdateUserAsync(userId, userDto);

        //    // Assert
        //    Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        //    Assert.Equal("User not found", result.Message);
        //} 

        [Fact]
        public async Task DeleteUserAsync_UserNotFound_ReturnsBadRequest()
        {
            // Arrange
            var userId = "1";
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

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
    }
}
