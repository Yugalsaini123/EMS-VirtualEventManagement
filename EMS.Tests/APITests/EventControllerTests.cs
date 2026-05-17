// EMS.Tests\APITests\EventControllerTests.cs

using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EMS.API.Controllers;
using EMS.Services.Interfaces;
using EMS.Services.Helpers;
using EMS.DAL.Models;
using Microsoft.Extensions.Logging;
 
namespace EMS.Tests.APITests
{
    [TestFixture]
    public class EventControllerTests
    {
        private Mock<IEventService> _mockService;
        private Mock<ILogger<EventsController>> _mockLogger;
        private EventsController _controller;
 
        [SetUp]
        public void Setup()
        {
            _mockService = new Mock<IEventService>();
            _mockLogger = new Mock<ILogger<EventsController>>();
            _controller = new EventsController(_mockService.Object, _mockLogger.Object);
        }
 
        [Test]
        public async Task GetAll_ShouldReturn200Ok()
        {
            // Arrange
            var mockResponse = new PaginatedResponse<EventListDto>
            {
                Data = new List<EventListDto>(),
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10,
                TotalPages = 0
            };
 
            _mockService.Setup(x => x.GetAllEventsAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(mockResponse);
 
            // Act
            var result = await _controller.GetAll();
 
            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }
 
        [Test]
        public async Task GetById_WithInvalidId_ShouldReturn404NotFound()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            _mockService.Setup(x => x.GetEventByIdAsync(invalidId))
                .ThrowsAsync(new KeyNotFoundException());
 
            // Act
            var result = await _controller.GetById(invalidId);
 
            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
 
        [Test]
        public async Task GetById_WithValidId_ShouldReturn200Ok()
        {
            // Arrange
            var eventId = Guid.NewGuid();
            var mockEvent = new
            {
                EventId = eventId,
                EventName = "Test Event",
                Status = "Active"
            };
 
            _mockService.Setup(x => x.GetEventByIdAsync(eventId))
                .ReturnsAsync(mockEvent);
 
            // Act
            var result = await _controller.GetById(eventId);
 
            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}