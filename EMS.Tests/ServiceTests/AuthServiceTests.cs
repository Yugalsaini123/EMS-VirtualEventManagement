// EMS.Tests\ServiceTests\AuthServiceTests.cs

using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using EMS.Services.Implementations;
using EMS.Services.Helpers;
using EMS.DAL.Repository;
using EMS.DAL.Models;
 
namespace EMS.Tests.ServiceTests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<IUserRepository> _mockRepository;
        private Mock<JwtTokenHelper> _mockJwtHelper;
        private AuthService _authService;
 
        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IUserRepository>();
            _mockJwtHelper = new Mock<JwtTokenHelper>(
                "YourVerySecureSecretKeyThatIsAtLeast32CharactersLongForHS256Algorithm",
                "EMSApi", "EMSApiUsers", 60);
            _authService = new AuthService(_mockRepository.Object, _mockJwtHelper.Object);
        }
 
        [Test]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var user = new UserInfo
            {
                EmailId = "admin@upgrad.com",
                UserName = "Admin",
                Password = "Admin@321",
                Role = "Admin"
            };
 
            _mockRepository.Setup(x => x.GetByEmailAsync("admin@upgrad.com"))
                .ReturnsAsync(user);
            _mockJwtHelper.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("fake-jwt-token");
 
            // Act
            var result = await _authService.LoginAsync("admin@upgrad.com", "Admin@321");
 
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("fake-jwt-token", result);
        }
 
        [Test]
        public void Login_WithInvalidPassword_ShouldThrowException()
        {
            // Arrange
            var user = new UserInfo
            {
                EmailId = "admin@upgrad.com",
                UserName = "Admin",
                Password = "Admin@321",
                Role = "Admin"
            };
 
            _mockRepository.Setup(x => x.GetByEmailAsync("admin@upgrad.com"))
                .ReturnsAsync(user);
 
            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _authService.LoginAsync("admin@upgrad.com", "wrongpassword"));
        }
 
        [Test]
        public void Login_WithNonexistentUser_ShouldThrowException()
        {
            // Arrange
            _mockRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((UserInfo)null);
 
            // Act & Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
                await _authService.LoginAsync("nonexistent@upgrad.com", "Admin@321"));
        }
 
        [Test]
        public async Task Register_WithValidData_ShouldSucceed()
        {
            // Arrange
            _mockRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((UserInfo)null);
            _mockRepository.Setup(x => x.AddAsync(It.IsAny<UserInfo>()))
                .Returns(Task.CompletedTask);
 
            // Act
            var result = await _authService.RegisterAsync("newuser@upgrad.com", "NewUser", "password123");
 
            // Assert
            Assert.IsTrue(result);
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<UserInfo>()), Times.Once);
        }
 
        [Test]
        public void Register_WithDuplicateEmail_ShouldThrowException()
        {
            // Arrange
            var existingUser = new UserInfo { EmailId = "test@upgrad.com" };
            _mockRepository.Setup(x => x.GetByEmailAsync("test@upgrad.com"))
                .ReturnsAsync(existingUser);
 
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _authService.RegisterAsync("test@upgrad.com", "User", "password123"));
        }
    }
}