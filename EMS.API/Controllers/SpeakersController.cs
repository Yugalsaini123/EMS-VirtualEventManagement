// EMS.API\Controllers\SpeakersController.cs
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
    public class SpeakersController : ControllerBase
    {
        private readonly ISpeakerService _speakerService;
        private readonly ILogger<SpeakersController> _logger;
 
        public SpeakersController(ISpeakerService speakerService, ILogger<SpeakersController> logger)
        {
            _speakerService = speakerService;
            _logger = logger;
        }
 
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all speakers");
 
                var result = await _speakerService.GetAllSpeakersAsync();
 
                return Ok(ResponseHelper.SuccessResponse(result, "Speakers retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting speakers");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving speakers", ex.Message, 400));
            }
        }
 
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation($"Getting speaker {id}");
 
                var result = await _speakerService.GetSpeakerByIdAsync(id);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Speaker retrieved successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Speaker not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting speaker {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving speaker", ex.Message, 400));
            }
        }
 
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSpeakerRequest request)
        {
            try
            {
                _logger.LogInformation($"Creating speaker: {request.SpeakerName}");
        
                if (!ModelState.IsValid)
                    return BadRequest(ResponseHelper.ErrorResponse("Invalid request", "Model validation failed", 400));
        
                var result = await _speakerService.CreateSpeakerAsync(
                request.SpeakerName, request.Email, request.Designation, request.Organization,
                request.Bio, request.PhoneNumber, request.LinkedInUrl);

        
                return CreatedAtAction(nameof(GetById), new { id = result.SpeakerId },
                    ResponseHelper.SuccessResponse(result, "Speaker created successfully", 201));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating speaker");
                return BadRequest(ResponseHelper.ErrorResponse("Error creating speaker", ex.Message, 400));
            }
        }
 
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSpeakerRequest request)
        {
            try
            {
                _logger.LogInformation($"Updating speaker {id}");
        
                if (!ModelState.IsValid)
                    return BadRequest(ResponseHelper.ErrorResponse("Invalid request", "Model validation failed", 400));
        
                var result = await _speakerService.UpdateSpeakerAsync(
                id, request.SpeakerName, request.Email, request.Designation, request.Organization,
                request.Bio, request.PhoneNumber, request.LinkedInUrl);

        
                return Ok(ResponseHelper.SuccessResponse(result, "Speaker updated successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Speaker not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating speaker {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error updating speaker", ex.Message, 400));
            }
        }
 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation($"Deleting speaker {id}");
 
                await _speakerService.DeleteSpeakerAsync(id);
 
                return Ok(ResponseHelper.SuccessResponse<object>(null, "Speaker deleted successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("Speaker not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting speaker {id}");
                return BadRequest(ResponseHelper.ErrorResponse("Error deleting speaker", ex.Message, 400));
            }
        }
 
        [HttpGet("active/all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetActiveSpeakers()
        {
            try
            {
                var result = await _speakerService.GetActiveSpeakersAsync();
 
                return Ok(ResponseHelper.SuccessResponse(result, "Active speakers retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active speakers");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving active speakers", ex.Message, 400));
            }
        }
 
        [HttpGet("search/{name}")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchByName(string name)
        {
            try
            {
                var result = await _speakerService.SearchSpeakersByNameAsync(name);
 
                return Ok(ResponseHelper.SuccessResponse(result, "Speakers found successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching speakers by name {name}");
                return BadRequest(ResponseHelper.ErrorResponse("Error searching speakers", ex.Message, 400));
            }
        }

        [HttpGet("workload/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSpeakersWithSessionCount()
        {
            try
            {
                _logger.LogInformation("Admin: Getting speakers with session count");

                var result = await _speakerService.GetSpeakersWithSessionCountAsync();

                return Ok(ResponseHelper.SuccessResponse(result, "Speaker workload retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting speaker workload");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving speaker workload", ex.Message, 400));
            }
        }
    }
}