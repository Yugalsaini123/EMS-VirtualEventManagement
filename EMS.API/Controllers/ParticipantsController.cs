// EMS.API\Controllers\ParticipantsController.cs

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
    [Authorize]
    public class ParticipantsController : ControllerBase
    {
        private readonly IParticipantService _participantService;
        private readonly ILogger<ParticipantsController> _logger;
 
        public ParticipantsController(IParticipantService participantService, ILogger<ParticipantsController> logger)
        {
            _participantService = participantService;
            _logger = logger;
        }
 
        [HttpPost("register")]
        [Authorize(Roles = "Participant,Admin")]
        public async Task<IActionResult> RegisterForEvent([FromBody] RegisterForEventRequest request)
        {
            try
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
 
                _logger.LogInformation($"Participant {email} registering for event {request.EventId}");
 
                if (!ModelState.IsValid)
                    return BadRequest(ResponseHelper.ErrorResponse("Invalid request", "Model validation failed", 400));
 
                var result = await _participantService.RegisterForEventAsync(email, request.EventId);
 
                return Ok(ResponseHelper.SuccessResponse(
                    new { registered = result, eventId = request.EventId },
                    "Successfully registered for event", 200));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Registration failed: {ex.Message}");
                return BadRequest(ResponseHelper.ErrorResponse("Registration failed", ex.Message, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering for event");
                return BadRequest(ResponseHelper.ErrorResponse("Error registering for event", ex.Message, 400));
            }
        }
 
        [HttpGet("my-events")]
        [Authorize(Roles = "Participant,Admin")]
        public async Task<IActionResult> GetMyEvents()
        {
            try
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
 
                _logger.LogInformation($"Getting events for participant {email}");
 
                var result = await _participantService.GetRegisteredEventsAsync(email);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Registered events retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving registered events");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving events", ex.Message, 400));
            }
        }
 
        [HttpPost("{id}/mark-attendance")]
        [Authorize(Roles = "Participant,Admin")]
        public async Task<IActionResult> MarkAttendance(Guid id)
        {
            try
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                _logger.LogInformation($"Marking attendance for registration {id} by {email}");

                if (!User.IsInRole("Admin"))
                {
                    var registration = await _participantService.GetRegistrationByIdAsync(id);
                    if (registration == null)
                        return NotFound(ResponseHelper.NotFoundResponse("Registration not found"));

                    if (!string.Equals(registration.ParticipantEmailId, email, StringComparison.OrdinalIgnoreCase))
                        return Forbid();
                }

                var result = await _participantService.MarkAttendanceAsync(id);

                return Ok(ResponseHelper.SuccessResponse(
                    new { marked = result },
                    "Attendance marked successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Registration not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking attendance for {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error marking attendance", ex.Message, 400));
            }
        }
 
        [HttpPost("{id}/feedback")]
        [Authorize(Roles = "Participant,Admin")]
        public async Task<IActionResult> SubmitFeedback(Guid id, [FromBody] SubmitFeedbackRequest request)
        {
            try
            {
                _logger.LogInformation($"Submitting feedback for registration {id}");
 
                if (!ModelState.IsValid)
                    return BadRequest(ResponseHelper.ErrorResponse("Invalid request", "Model validation failed", 400));
 
                var result = await _participantService.SubmitFeedbackAsync(id, request.Rating, request.Feedback);
 
                return Ok(ResponseHelper.SuccessResponse(
                    new { submitted = result },
                    "Feedback submitted successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Registration not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error submitting feedback for {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error submitting feedback", ex.Message, 400));
            }
        }
 
        [HttpPost("unregister")]
        [Authorize(Roles = "Participant,Admin")]
        public async Task<IActionResult> UnregisterFromEvent([FromBody] UnregisterRequest request)
        {
            try
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
 
                _logger.LogInformation($"Participant {email} unregistering from event {request.EventId}");
 
                var result = await _participantService.UnregisterFromEventAsync(email, request.EventId);
 
                return Ok(ResponseHelper.SuccessResponse(
                    new { unregistered = result },
                    "Successfully unregistered from event", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unregistering from event");
                return BadRequest(ResponseHelper.ErrorResponse("Error unregistering", ex.Message, 400));
            }
        }
 
        [HttpGet("event/{eventId}/participants")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEventParticipants(Guid eventId)
        {
            try
            {
                _logger.LogInformation($"Getting participants for event {eventId}");
 
                var result = await _participantService.GetEventParticipantsAsync(eventId);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Participants retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting participants for event {eventId}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving participants", ex.Message, 400));
            }
        }
 
        [HttpGet("check-registration/{eventId}")]
        [Authorize(Roles = "Participant,Admin")]
        public async Task<IActionResult> CheckRegistration(Guid eventId)
        {
            try
            {
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
 
                _logger.LogInformation($"Checking registration for {email} in event {eventId}");
 
                var result = await _participantService.IsRegisteredAsync(email, eventId);
 
                return Ok(ResponseHelper.SuccessResponse(
                    new { isRegistered = result },
                    "Registration status retrieved", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking registration for event {eventId}");
                return BadRequest(ResponseHelper.ErrorResponse("Error checking registration", ex.Message, 400));
            }
        }

        [HttpGet("event/{eventId}/stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAttendanceStats(Guid eventId)
        {
            try
            {
                _logger.LogInformation($"Admin: Getting attendance stats for event {eventId}");

                var result = await _participantService.GetAttendanceStatsAsync(eventId);

                return Ok(ResponseHelper.SuccessResponse(result, "Attendance statistics retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting attendance stats for event {eventId}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving attendance stats", ex.Message, 400));
            }
        }
    }
}