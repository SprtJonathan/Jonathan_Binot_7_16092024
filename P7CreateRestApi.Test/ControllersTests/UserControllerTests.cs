using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7CreateRestApi.Tests.ControllersTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole<int>>> _roleManagerMock;
        private readonly Mock<ILogger<UserController>> _loggerMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userManagerMock = MockUserManager();
            _roleManagerMock = MockRoleManager();
            _loggerMock = new Mock<ILogger<UserController>>();
            _controller = new UserController(_userRepositoryMock.Object, _userManagerMock.Object, _roleManagerMock.Object, _loggerMock.Object);
        }

        private Mock<UserManager<User>> MockUserManager()
        {
            return new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        }

        private Mock<RoleManager<IdentityRole<int>>> MockRoleManager()
        {
            return new Mock<RoleManager<IdentityRole<int>>>(
                Mock.Of<IRoleStore<IdentityRole<int>>>(), null, null, null, null);
        }

        // Test pour GetAllUsers - Cas où des utilisateurs existent
        [Fact]
        public async Task GetAllUsers_ShouldReturnOk_WhenUsersExist()
        {
            // Arrange
            var users = new List<User> { new User { Id = 1, UserName = "user1" }, new User { Id = 2, UserName = "user2" } };
            _userRepositoryMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsType<List<User>>(actionResult.Value);
            Assert.Equal(users.Count, returnedUsers.Count);
        }

        // Test pour GetAllUsers - Cas où aucun utilisateur n'est trouvé
        [Fact]
        public async Task GetAllUsers_ShouldReturnOk_WhenNoUsersExist()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsType<List<User>>(actionResult.Value);
            Assert.Empty(returnedUsers);
        }

        // Test pour GetUserById - Cas de succès
        [Fact]
        public async Task GetUserById_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "user1" };
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, actionResult.Value);
        }

        // Test pour GetUserById - Cas de l'utilisateur non trouvé
        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour Register - Cas de succès
        [Fact]
        public async Task Register_ShouldReturnCreatedAtAction_WhenUserIsValid()
        {
            // Arrange
            var registerModel = new RegisterModel { UserName = "user1", Password = "Password123!", Role = "User" };
            var user = new User { Id = 1, UserName = "user1" };

            _roleManagerMock.Setup(r => r.RoleExistsAsync(registerModel.Role)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), registerModel.Password))
                            .ReturnsAsync(IdentityResult.Success)
                            .Callback<User, string>((createdUser, _) => createdUser.Id = 1);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), registerModel.Role)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetUserById", actionResult.ActionName);
            Assert.Equal(1, actionResult.RouteValues["id"]);
        }

        // Test pour Register - Cas de rôle non existant
        [Fact]
        public async Task Register_ShouldReturnInternalServerError_WhenRoleCreationFails()
        {
            // Arrange
            var registerModel = new RegisterModel { UserName = "user1", Password = "Password123!", Role = "NonExistentRole" };

            _roleManagerMock.Setup(r => r.RoleExistsAsync(registerModel.Role)).ReturnsAsync(false);
            _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole<int>>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Role creation failed" }));

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var actionResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, actionResult.StatusCode);
        }

        // Test pour UpdateUser - Cas de succès
        [Fact]
        public async Task UpdateUser_ShouldReturnOk_WhenUserExistsAndIsUpdated()
        {
            // Arrange
            var registerModel = new RegisterModel { UserName = "user1", Password = "Password123!", Role = "User" };
            var user = new User { Id = 1, UserName = "user1" };

            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.UpdateUser(1, registerModel);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, actionResult.Value);
        }

        // Test pour UpdateUser - Cas de l'utilisateur non trouvé
        [Fact]
        public async Task UpdateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var registerModel = new RegisterModel { UserName = "user1", Password = "Password123!", Role = "User" };

            _userManagerMock.Setup(u => u.FindByIdAsync("1")).ReturnsAsync((User)null);

            // Act
            var result = await _controller.UpdateUser(1, registerModel);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour DeleteUser - Cas de succès
        [Fact]
        public async Task DeleteUser_ShouldReturnNoContent_WhenUserExists()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "user1" };
            _userRepositoryMock.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.DeleteUserAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Test pour DeleteUser - Cas de l'utilisateur non trouvé
        [Fact]
        public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.DeleteUserAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
