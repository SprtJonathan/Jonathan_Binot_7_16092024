﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Models;
using P7CreateRestApi.Services;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace P7CreateRestApi.Tests.ControllersTests
{
    public class LoginControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly Mock<ILogger<LoginController>> _mockLogger;
        private readonly LoginController _controller;

        public LoginControllerTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null, null, null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null, null, null, null);

            _mockJwtService = new Mock<IJwtService>();
            _mockLogger = new Mock<ILogger<LoginController>>();
            _controller = new LoginController(_mockSignInManager.Object, _mockJwtService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                UserName = "validUser",
                Password = "validPassword",
                RememberMe = true
            };

            var user = new User { UserName = "validUser" };
            var jwtToken = "valid_token";

            _mockSignInManager
                .Setup(s => s.PasswordSignInAsync(loginModel.UserName, loginModel.Password, loginModel.RememberMe, true))
                .ReturnsAsync(SignInResult.Success);

            _mockUserManager
                .Setup(u => u.FindByNameAsync(loginModel.UserName))
                .ReturnsAsync(user);

            _mockJwtService
                .Setup(j => j.GenerateToken(user))
                .ReturnsAsync(jwtToken);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse>(okResult.Value);

            Assert.Equal("Connexion réussie.", response.Message);
            Assert.Equal(jwtToken, response.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                UserName = "invalidUser",
                Password = "invalidPassword",
                RememberMe = false
            };

            _mockSignInManager.Setup(s => s.PasswordSignInAsync(loginModel.UserName, loginModel.Password, loginModel.RememberMe, true))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);

            Assert.Equal("Nom d'utilisateur ou mot de passe incorrect.", response.Message);
            Assert.Null(response.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenAccountIsLockedOut()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                UserName = "lockedOutUser",
                Password = "password",
                RememberMe = false
            };

            _mockSignInManager.Setup(s => s.PasswordSignInAsync(loginModel.UserName, loginModel.Password, loginModel.RememberMe, true))
                .ReturnsAsync(SignInResult.LockedOut);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ApiResponse>(unauthorizedResult.Value);

            Assert.Equal("Compte verrouillé en raison de tentatives de connexion infructueuses.", response.Message);
            Assert.Null(response.Token);
        }

        [Fact]
        public async Task Logout_ShouldReturnOk()
        {
            // Arrange
            _mockSignInManager.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse>(okResult.Value);

            Assert.Equal("Déconnexion réussie.", response.Message);
            Assert.Null(response.Token);
        }
    }
}
