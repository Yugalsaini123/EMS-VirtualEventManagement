// EMS.Tests/DALTests/SessionRepositoryTests.cs
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
    public class SessionRepositoryTests
    {
        private ISessionRepository _repository;
        private EMSContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<EMSContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new EMSContext(options);
            _repository = new SessionRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddSession_ShouldInsertSuccessfully()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var newSession = new SessionInfo
            {
                SessionId = sessionId,
                EventId = Guid.NewGuid(),
                SessionTitle = "Test Session",
                Description = "Session Description",        
                SessionUrl = "https://meet.example.com/test", 
                SessionStart = DateTime.UtcNow.AddHours(2),
                SessionEnd = DateTime.UtcNow.AddHours(3),
                Location = "Room 101",
                Status = "Scheduled",
                CreatedDate = DateTime.UtcNow
            };

            // Act
            await _repository.AddAsync(newSession);
            var result = await _repository.GetByIdAsync(sessionId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Session", result.SessionTitle);
        }

        [Test]
        public async Task DeleteSession_ShouldRemoveRecord()
        {
            // Arrange
            var sessionToDelete = new SessionInfo
            {
                SessionId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                SessionTitle = "Delete This",
                Description = "To be deleted",                 
                SessionUrl = "https://meet.example.com/delete", 
                SessionStart = DateTime.UtcNow.AddHours(1),
                SessionEnd = DateTime.UtcNow.AddHours(2),
                Location = "Room",
                Status = "Scheduled",
                CreatedDate = DateTime.UtcNow
            };
            await _repository.AddAsync(sessionToDelete);

            // Act
            await _repository.RemoveAsync(sessionToDelete);
            var result = await _repository.GetByIdAsync(sessionToDelete.SessionId);

            // Assert
            Assert.IsNull(result);
        }
    }
}