// EMS.Tests/DALTests/UserRepositoryTests.cs
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using EMS.DAL.Models;
using EMS.DAL.Repository;
using EMS.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.Tests.DALTests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private IUserRepository _repository;
        private EMSContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<EMSContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new EMSContext(options);
            _repository = new UserRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetUserByEmail_ShouldReturnCorrectUser()
        {
            await _repository.AddAsync(new UserInfo
            {
                EmailId = "test@example.com", UserName = "TestUser",
                Password = "password123", Role = "Participant",
                IsActive = true, CreatedAt = DateTime.UtcNow
            });

            var result = await _repository.GetByEmailAsync("test@example.com");

            Assert.IsNotNull(result);
            Assert.AreEqual("TestUser", result.UserName);
        }

        [Test]
        public async Task AddUser_ShouldInsertSuccessfully()
        {
            var newUser = new UserInfo
            {
                EmailId = "newuser@example.com", UserName = "NewUser",
                Password = "secure123", Role = "Participant",
                IsActive = true, CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(newUser);
            var result = await _repository.GetByEmailAsync("newuser@example.com");

            Assert.IsNotNull(result);
            Assert.AreEqual("NewUser", result.UserName);
        }

        [Test]
        public async Task DeleteUser_ShouldRemoveRecord()
        {
            var userToDelete = new UserInfo
            {
                EmailId = "delete@example.com", UserName = "DeleteMe",
                Password = "pass123", Role = "Participant",
                IsActive = true, CreatedAt = DateTime.UtcNow
            };
            await _repository.AddAsync(userToDelete);

            await _repository.RemoveAsync(userToDelete);
            var result = await _repository.GetByEmailAsync("delete@example.com");

            Assert.IsNull(result);
        }
    }
}