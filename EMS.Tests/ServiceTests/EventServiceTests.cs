// EMS.Tests\ServiceTests\EventServiceTests.cs

using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Services.Implementations;
using EMS.Services.Helpers;
using EMS.DAL.Repository;
using EMS.DAL.Models;
using Microsoft.Extensions.Caching.Memory;
 
namespace EMS.Tests.ServiceTests
{
    [TestFixture]
    public class EventServiceTests
    {
        private Mock<IEventRepository> _mockRepository;
        private Mock<IMemoryCache> _mockCache;
        private EventService _eventService;
 
        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IEventRepository>();
            _mockCache = new Mock<IMemoryCache>();
            _eventService = new EventService(_mockRepository.Object, _mockCache.Object);
        }
 
        [Test]
        public async Task GetAllEvents_ShouldReturnPaginatedList()
        {
            // Arrange
            var mockEvents = new List<EventDetails>
            {
                new EventDetails 
                { 
                    EventId = Guid.NewGuid(), 
                    EventName = "Event 1", 
                    EventCategory = "Tech",
                    EventDate = DateTime.UtcNow.AddDays(1),
                    Status = "Active",
                    Location = "City"
                },
                new EventDetails 
                { 
                    EventId = Guid.NewGuid(), 
                    EventName = "Event 2", 
                    EventCategory = "Business",
                    EventDate = DateTime.UtcNow.AddDays(2),
                    Status = "Active",
                    Location = "State"
                }
            };
 
            _mockRepository.Setup(x => x.GetEventsWithParticipantCountAsync())
                .ReturnsAsync(mockEvents.Select(e => new EventListDto
                {
                    EventId = e.EventId,
                    EventName = e.EventName,
                    EventCategory = e.EventCategory,
                    EventDate = e.EventDate,
                    Status = e.Status,
                    Location = e.Location,
                    MaxParticipants = e.MaxParticipants,
                    ParticipantCount = 0,
                    SessionCount = 0
                }).ToList().AsEnumerable());
 
            // Act
            var result = await _eventService.GetAllEventsAsync(1, 10);
 
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            _mockRepository.Verify(x => x.GetEventsWithParticipantCountAsync(), Times.Once);
        }
 
        [Test]
        public async Task CreateEvent_WithValidData_ShouldSucceed()
        {
            // Arrange
            _mockRepository.Setup(x => x.AddAsync(It.IsAny<EventDetails>()))
                .Returns(Task.CompletedTask);
            _mockCache.Setup(x => x.Remove(It.IsAny<string>()));
 
            // Act
            var result = await _eventService.CreateEventAsync(
                "Conference 2024", "Technology", DateTime.UtcNow.AddDays(30),
                "Annual tech conference", "Convention Center", 500);
 
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Conference 2024", result.EventName);
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<EventDetails>()), Times.Once);
        }
 
        [Test]
        public void CreateEvent_WithPastDate_ShouldThrowException()
        {
            // Arrange & Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _eventService.CreateEventAsync(
                    "Past Event", "Tech", DateTime.UtcNow.AddDays(-1),
                    "Oops", "Nowhere", 100));
        }
 
        [Test]
        public void CreateEvent_WithoutName_ShouldThrowException()
        {
            // Arrange & Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _eventService.CreateEventAsync(
                    "", "Tech", DateTime.UtcNow.AddDays(1),
                    "No name", "Venue", 100));
        }
 
        [Test]
        public async Task DeleteEvent_ShouldRemoveRecord()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var mockEvent = new EventDetails 
            { 
                EventId = eventId, 
                EventName = "To Delete",
                EventCategory = "Tech",
                Status = "Active"
            };
 
            _mockRepository.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync(mockEvent);
            _mockRepository.Setup(x => x.RemoveAsync(It.IsAny<EventDetails>()))
                .Returns(Task.CompletedTask);
            _mockCache.Setup(x => x.Remove(It.IsAny<string>()));
 
            // Act
            var result = await _eventService.DeleteEventAsync(eventId);
 
            // Assert
            Assert.IsTrue(result);
            _mockRepository.Verify(x => x.RemoveAsync(It.IsAny<EventDetails>()), Times.Once);
        }
 
        [Test]
        public void DeleteEvent_NonexistentEvent_ShouldThrowException()
        {
            // Arrange
            _mockRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((EventDetails)null);
 
            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(async () =>
                await _eventService.DeleteEventAsync(Guid.NewGuid()));
        }
    }
}