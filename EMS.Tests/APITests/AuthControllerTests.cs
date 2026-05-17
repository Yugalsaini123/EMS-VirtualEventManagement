// EMS.Tests\APITests\AuthControllerTests.cs

using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMS.API.Controllers;
using EMS.Services.Interfaces;
using EMS.API.Models.Requests;
using Microsoft.Extensions.Logging;
 
namespace EMS.Tests.APITests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockService;
        private Mock<ILogger<AuthController>> _mockLogger;
        private AuthController _controller;
 
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockService.Object, _mockLogger.Object);
        }
 
        [Test]
        public async Task Login_WithValidCredentials_ShouldReturn200Ok()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "admin@upgrad.com",
                Password = "Admin@321"
            };
 
            _mockService.Setup(x => x.LoginAsync(request.Email, request.Password))
                .ReturnsAsync("fake-jwt-token");
 
            // Act
            var result = await _controller.Login(request);
 
            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
 
        [Test]
        public async Task Login_WithInvalidCredentials_ShouldReturn401Unauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "admin@upgrad.com",
                Password = "wrongpassword"
            };
 
            _mockService.Setup(x => x.LoginAsync(request.Email, request.Password))
                .ThrowsAsync(new UnauthorizedAccessException());
 
            // Act
            var result = await _controller.Login(request);
 
            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }
 
        [Test]
        public async Task Register_WithValidData_ShouldReturn200Ok()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = "newuser@upgrad.com",
                UserName = "NewUser",
                Password = "password123",
                ConfirmPassword = "password123"
            };
 
            _mockService.Setup(x => x.RegisterAsync(request.Email, request.UserName, request.Password))
                .ReturnsAsync(true);
 
            // Act
            var result = await _controller.Register(request);
 
            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}