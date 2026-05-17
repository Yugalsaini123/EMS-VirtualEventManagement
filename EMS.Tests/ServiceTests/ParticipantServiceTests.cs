// EMS.Tests\ServiceTests\ParticipantServiceTests.cs

using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMS.Services.Implementations;
using EMS.DAL.Repository;
using EMS.DAL.Models;
 
namespace EMS.Tests.ServiceTests
{
    [TestFixture]
    public class ParticipantServiceTests
    {
        private Mock<IParticipantEventRepository> _mockRepository;
        private Mock<IEventRepository> _mockEventRepository;
        private ParticipantService _participantService;
 
        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IParticipantEventRepository>();
            _mockEventRepository = new Mock<IEventRepository>();
            _participantService = new ParticipantService(_mockRepository.Object, _mockEventRepository.Object);
        }
 
        [Test]
        public async Task RegisterForEvent_WithValidData_ShouldSucceed()
        {
            // Arrange
            var email = "participant@upgrad.com";
            var eventId = Guid.NewGuid();
 
            _mockRepository.Setup(x => x.IsRegisteredAsync(email, eventId))
                .ReturnsAsync(false);
            _mockRepository.Setup(x => x.AddAsync(It.IsAny<ParticipantEventDetails>()))
                .Returns(Task.CompletedTask);
 
            // Act
            var result = await _participantService.RegisterForEventAsync(email, eventId);
 
            // Assert
            Assert.IsTrue(result);
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<ParticipantEventDetails>()), Times.Once);
        }
 
        [Test]
        public void RegisterForEvent_AlreadyRegistered_ShouldThrowException()
        {
            // Arrange
            var email = "participant@upgrad.com";
            var eventId = Guid.NewGuid();
 
            _mockRepository.Setup(x => x.IsRegisteredAsync(email, eventId))
                .ReturnsAsync(true);
 
            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _participantService.RegisterForEventAsync(email, eventId));
        }
 
        [Test]
        public async Task GetRegisteredEvents_ShouldReturnList()
        {
            // Arrange
            var email = "participant@upgrad.com";
            var registrations = new List<ParticipantEventDetails>
            {
                new ParticipantEventDetails
                {
                    Id = Guid.NewGuid(),
                    ParticipantEmailId = email,
                    EventId = Guid.NewGuid(),
                    RegistrationDate = DateTime.UtcNow
                }
            };
 
            _mockRepository.Setup(x => x.GetRegisteredEventsAsync(email))
                .ReturnsAsync(registrations);
 
            // Act
            var result = await _participantService.GetRegisteredEventsAsync(email);
 
            // Assert
            Assert.IsNotNull(result);
            Assert.GreaterOrEqual(result.Count, 0);
        }
 
        [Test]
        public async Task MarkAttendance_ShouldSucceed()
        {
            // Arrange
            var registrationId = Guid.NewGuid();
            var registration = new ParticipantEventDetails
            {
                Id = registrationId,
                IsAttended = false
            };
 
            _mockRepository.Setup(x => x.GetByIdAsync(registrationId))
                .ReturnsAsync(registration);
            _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<ParticipantEventDetails>()))
                .Returns(Task.CompletedTask);
 
            // Act
            var result = await _participantService.MarkAttendanceAsync(registrationId);
 
            // Assert
            Assert.IsTrue(result);
        }
    }
}