// EMS.API\Controllers\SessionsController.cs
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
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly ILogger<SessionsController> _logger;
 
        public SessionsController(ISessionService sessionService, ILogger<SessionsController> logger)
        {
            _sessionService = sessionService;
            _logger = logger;
        }
 
        [HttpGet("event/{eventId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByEvent(Guid eventId)
        {
            try
            {
                _logger.LogInformation($"Getting sessions for event {eventId}");
 
                var result = await _sessionService.GetSessionsByEventAsync(eventId);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Sessions retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting sessions for event {eventId}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving sessions", ex.Message, 400));
            }
        }

        [HttpGet("event/{eventId}/search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchByEvent(Guid eventId, [FromQuery] string searchTerm)
        {
            try
            {
                _logger.LogInformation($"Searching sessions for event {eventId} with term: {searchTerm}");
 
                var result = await _sessionService.SearchSessionsByEventAsync(eventId, searchTerm);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Sessions retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching sessions for event {eventId}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving sessions", ex.Message, 400));
            }
        }
 
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation($"Getting session {id}");
 
                var result = await _sessionService.GetSessionByIdAsync(id);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Session retrieved successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Session not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting session {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving session", ex.Message, 400));
            }
        }
 
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSessionRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating session: {request.Title}");
 
                if (!ModelState.IsValid)
                    return BadRequest(ResponseHelper.ErrorResponse("Invalid request", "Model validation failed", 400));
 
                var result = await _sessionService.CreateSessionAsync(
                    request.EventId, request.Title, request.StartTime, request.EndTime,
                    request.Description, request.Location, request.SpeakerId, request.SessionUrl);
 
                return CreatedAtAction(nameof(GetById), new { id = result.SessionId },
                    ResponseHelper.SuccessResponse(result, "Session created successfully", 201));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid session data: {ex.Message}");
                return BadRequest(ResponseHelper.ErrorResponse("Invalid session data", ex.Message, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating session");
                return BadRequest(ResponseHelper.ErrorResponse("Error creating session", ex.Message, 400));
            }
        }
 
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSessionRequest request)
        {
            try
            {
                _logger.LogInformation($"Updating session {id}");
 
                if (!ModelState.IsValid)
                    return BadRequest(ResponseHelper.ErrorResponse("Invalid request", "Model validation failed", 400));
 
                var result = await _sessionService.UpdateSessionAsync(
                    id, request.Title, request.StartTime, request.EndTime,
                    request.Description, request.Location, request.SpeakerId, request.SessionUrl);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Session updated successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Session not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating session {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error updating session", ex.Message, 400));
            }
        }
 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation($"Deleting session {id}");
 
                await _sessionService.DeleteSessionAsync(id);
 
                return Ok(ResponseHelper.SuccessResponse<object>(null, "Session deleted successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Session not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting session {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error deleting session", ex.Message, 400));
            }
        }
 
        [HttpPost("{id}/assign-speaker")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignSpeaker(Guid id, [FromBody] AssignSpeakerRequest request)
        {
            try
            {
                _logger.LogInformation($"Assigning speaker {request.SpeakerId} to session {id}");
 
                var result = await _sessionService.AssignSpeakerAsync(id, request.SpeakerId);
 
                return Ok(ResponseHelper.SuccessResponse(new { assigned = result }, "Speaker assigned successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning speaker to session {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error assigning speaker", ex.Message, 400));
            }
        }
 
        [HttpPost("{id}/remove-speaker")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveSpeaker(Guid id)
        {
            try
            {
                _logger.LogInformation($"Removing speaker from session {id}");
 
                var result = await _sessionService.RemoveSpeakerAsync(id);
 
                return Ok(ResponseHelper.SuccessResponse(new { removed = result }, "Speaker removed successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing speaker from session {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error removing speaker", ex.Message, 400));
            }
        }
    }
}