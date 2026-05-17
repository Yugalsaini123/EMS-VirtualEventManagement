// EMS.Tests/DALTests/EventRepositoryTests.cs
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using EMS.DAL.Models;
using EMS.DAL.Repository;
using EMS.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.Tests.DALTests
{
    [TestFixture]
    public class EventRepositoryTests
    {
        private IEventRepository _repository;
        private EMSContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<EMSContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new EMSContext(options);
            _repository = new EventRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task GetAllEvents_ShouldReturnListOfEvents()
        {
            // Arrange
            await _repository.AddAsync(new EventDetails
            {
                EventId = Guid.NewGuid(),
                EventName = "Seed Event",
                EventCategory = "Tech",
                EventDate = DateTime.UtcNow.AddDays(1),
                Description = "Seed Description",   
                Location = "City",
                Status = "Active",
                CreatedDate = DateTime.UtcNow
            });

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.GreaterOrEqual(result.Count(), 1);
        }

        [Test]
        public async Task AddEvent_ShouldInsertSuccessfully()
        {
            // Arrange
            var newEvent = new EventDetails
            {
                EventId = Guid.NewGuid(),
                EventName = "Test Event",
                EventCategory = "Technology",
                EventDate = DateTime.UtcNow.AddDays(5),
                Description = "Test Description",  
                Location = "Test Location",
                Status = "Active",
                CreatedDate = DateTime.UtcNow
            };

            // Act
            await _repository.AddAsync(newEvent);
            var retrievedEvent = await _repository.GetByIdAsync(newEvent.EventId);

            // Assert
            Assert.IsNotNull(retrievedEvent);
            Assert.AreEqual("Test Event", retrievedEvent.EventName);
        }

        [Test]
        public async Task DeleteEvent_ShouldRemoveRecord()
        {
            // Arrange
            var eventToDelete = new EventDetails
            {
                EventId = Guid.NewGuid(),
                EventName = "Event to Delete",
                EventCategory = "Tech",
                EventDate = DateTime.UtcNow.AddDays(1),
                Description = "To be deleted",     
                Location = "Test",
                Status = "Active",
                CreatedDate = DateTime.UtcNow
            };
            await _repository.AddAsync(eventToDelete);

            // Act
            await _repository.RemoveAsync(eventToDelete);
            var result = await _repository.GetByIdAsync(eventToDelete.EventId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetEventById_ShouldReturnCorrectEvent()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            await _repository.AddAsync(new EventDetails
            {
                EventId = eventId,
                EventName = "Specific Event",
                EventCategory = "Business",
                EventDate = DateTime.UtcNow.AddDays(3),
                Description = "Business conference",  
                Location = "City Hall",
                Status = "Active",
                CreatedDate = DateTime.UtcNow
            });

            // Act
            var result = await _repository.GetByIdAsync(eventId);

            // Assert
            Assert.AreEqual("Specific Event", result.EventName);
            Assert.AreEqual("Business", result.EventCategory);
        }

        [Test]
        public async Task UpdateEvent_ShouldModifyRecord()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            await _repository.AddAsync(new EventDetails
            {
                EventId = eventId,
                EventName = "Original Name",
                EventCategory = "Tech",
                EventDate = DateTime.UtcNow.AddDays(2),
                Description = "Original Description", 
                Location = "Original Location",
                Status = "Active",
                CreatedDate = DateTime.UtcNow
            });

            // Act
            var eventToUpdate = await _repository.GetByIdAsync(eventId);
            eventToUpdate.EventName = "Updated Name";
            eventToUpdate.Location = "Updated Location";
            await _repository.UpdateAsync(eventToUpdate);
            var updatedEvent = await _repository.GetByIdAsync(eventId);

            // Assert
            Assert.AreEqual("Updated Name", updatedEvent.EventName);
            Assert.AreEqual("Updated Location", updatedEvent.Location);
        }
    }
}