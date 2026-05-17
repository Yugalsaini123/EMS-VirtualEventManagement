// EMS.API\Controllers\EventsController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMS.Services.Interfaces;
using EMS.Services.Helpers;
using EMS.API.Models.Requests;
 
namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;
 
        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }
 
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
            [FromQuery] string search = null, [FromQuery] string sortBy = null, [FromQuery] string sortOrder = "asc")
        {
            try
            {
                _logger.LogInformation($"Getting events - Page {pageNumber}, Size {pageSize}, Search {search}, SortBy {sortBy}, SortOrder {sortOrder}");
 
                var result = await _eventService.GetAllEventsAsync(pageNumber, pageSize, search, sortBy, sortOrder);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Events retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting events");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving events", ex.Message, 400));
            }
        }
 
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation($"Getting event {id}");
 
                var result = await _eventService.GetEventByIdAsync(id);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Event retrieved successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Event not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting event {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving event", ex.Message, 400));
            }
        }
 
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating event: {request.EventName}");
 
                if (!ModelState.IsValid)
                    return BadRequest(ResponseHelper.ErrorResponse("Invalid request", "Model validation failed", 400));
 
                var result = await _eventService.CreateEventAsync(
                    request.EventName, request.EventCategory, request.EventDate,
                    request.Description, request.Location, request.MaxParticipants);
 
                return CreatedAtAction(nameof(GetById), new { id = result.EventId },
                    ResponseHelper.SuccessResponse(result, "Event created successfully", 201));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid event data: {ex.Message}");
                return BadRequest(ResponseHelper.ErrorResponse("Invalid event data", ex.Message, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                return BadRequest(ResponseHelper.ErrorResponse("Error creating event", ex.Message, 400));
            }
        }
 
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventRequest request)
        {
            try
            {
                _logger.LogInformation($"Updating event {id}");
 
                if (!ModelState.IsValid)
                    return BadRequest(ResponseHelper.ErrorResponse("Invalid request", "Model validation failed", 400));
 
                var result = await _eventService.UpdateEventAsync(
                    id, request.EventName, request.EventCategory, request.EventDate,
                    request.Description, request.Location, request.MaxParticipants);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Event updated successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Event not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating event {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error updating event", ex.Message, 400));
            }
        }
 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation($"Deleting event {id}");
 
                await _eventService.DeleteEventAsync(id);
 
                return Ok(ResponseHelper.SuccessResponse<object>(null, "Event deleted successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Event not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting event {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error deleting event", ex.Message, 400));
            }
        }
 
        [HttpGet("categories/all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                _logger.LogInformation("Getting event categories");
 
                var result = await _eventService.GetCategoriesAsync();
 
                return Ok(ResponseHelper.SuccessResponse(result, "Categories retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving categories", ex.Message, 400));
            }
        }
 
        [HttpGet("category/{category}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategory(string category)
        {
            try
            {
                var result = await _eventService.GetEventsByCategoryAsync(category);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Events retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting events for category {category}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving events", ex.Message, 400));
            }
        }
 
        [HttpGet("upcoming/all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUpcoming()
        {
            try
            {
                var result = await _eventService.GetUpcomingEventsAsync();
 
                return Ok(ResponseHelper.SuccessResponse(result, "Upcoming events retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upcoming events");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving upcoming events", ex.Message, 400));
            }
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEventsAdmin()
        {
            try
            {
                _logger.LogInformation("Admin: Getting all events (Active + Inactive)");

                var result = await _eventService.GetAllEventsAdminAsync();

                return Ok(ResponseHelper.SuccessResponse(result, "All events retrieved successfully (admin view)", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all events for admin");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving events", ex.Message, 400));
            }
        }

        [HttpPost("{id}/toggle-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleEventStatus(Guid id)
        {
            try
            {
                _logger.LogInformation($"Admin: Toggling status for event {id}");

                var result = await _eventService.ToggleEventStatusAsync(id);

                if (!result)
                    return NotFound(ResponseHelper.NotFoundResponse("Event not found"));

                return Ok(ResponseHelper.SuccessResponse(
                    new { toggled = result },
                    "Event status toggled successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling event status {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error toggling event status", ex.Message, 400));
            }
        }
    }
}