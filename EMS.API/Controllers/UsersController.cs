// EMS.API\Controllers\UsersController.cs
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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]             // admin only
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting all users");

                var result = await _userService.GetAllUsersAsync();

                return Ok(ResponseHelper.SuccessResponse(result, "Users retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving users", ex.Message, 400));
            }
        }

        [HttpGet("{email}")]
        [Authorize(Roles = "Admin")]             // admin only
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                _logger.LogInformation($"Getting user {email}");

                var result = await _userService.GetUserByEmailAsync(email);

                return Ok(ResponseHelper.SuccessResponse(result, "User retrieved successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("User not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user {email}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving user", ex.Message, 400));
            }
        }

        [HttpPut("{email}")]
        [Authorize(Roles = "Admin")]             // admin only
        public async Task<IActionResult> Update(string email, [FromBody] UpdateUserRequest request)
        {
            try
            {
                _logger.LogInformation($"Updating user {email}");

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ResponseHelper.ErrorResponse("Validation failed", errors, 400));
                }

                var result = await _userService.UpdateUserAsync(email, request.UserName, request.Password);

                return Ok(ResponseHelper.SuccessResponse(result, "User updated successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("User not found"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResponseHelper.ErrorResponse("Validation failed", ex.Message, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {email}");
                return BadRequest(ResponseHelper.ErrorResponse("Error updating user", ex.Message, 400));
            }
        }

        [HttpDelete("{email}")]
        [Authorize(Roles = "Admin")]             // admin only
        public async Task<IActionResult> Delete(string email)
        {
            try
            {
                _logger.LogInformation($"Deleting user {email}");

                await _userService.DeleteUserAsync(email);

                return Ok(ResponseHelper.SuccessResponse<object>(null, "User deleted successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("User not found"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseHelper.ErrorResponse("Cannot delete user", ex.Message, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {email}");
                return BadRequest(ResponseHelper.ErrorResponse("Error deleting user", ex.Message, 400));
            }
        }

        [HttpPut("{email}/change-password")]
        // no role restriction — any authenticated user reaches this
        // ownership is enforced inside the method
        public async Task<IActionResult> ChangePassword(string email, [FromBody] ChangePasswordRequest request)
        {
            try
            {
                var loggedInEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                                 ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // only the account owner can change their own password — no admin bypass
                if (string.IsNullOrEmpty(loggedInEmail) ||
                    !string.Equals(loggedInEmail, email, StringComparison.OrdinalIgnoreCase))
                {
                    return StatusCode(403, ResponseHelper.ErrorResponse(
                        "Forbidden",
                        "You can only change your own password",
                        403));
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ResponseHelper.ErrorResponse("Validation failed", errors, 400));
                }

                var result = await _userService.ChangePasswordAsync(email, request.OldPassword, request.NewPassword);

                return Ok(ResponseHelper.SuccessResponse(
                    new { changed = result },
                    "Password changed successfully", 200));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ResponseHelper.NotFoundResponse("User not found"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ResponseHelper.UnauthorizedResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ResponseHelper.ErrorResponse("Validation failed", ex.Message, 400));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ResponseHelper.ErrorResponse("Operation failed", ex.Message, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing password for {email}");
                return BadRequest(ResponseHelper.ErrorResponse("Error changing password", ex.Message, 400));
            }
        }

        [HttpGet("role/{role}")]
        [Authorize(Roles = "Admin")]             // admin only
        public async Task<IActionResult> GetByRole(string role)
        {
            try
            {
                _logger.LogInformation($"Getting users with role {role}");

                var result = await _userService.GetUsersByRoleAsync(role);

                return Ok(ResponseHelper.SuccessResponse(result, $"Users with role {role} retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting users with role {role}");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving users", ex.Message, 400));
            }
        }

        [HttpGet("participants/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllParticipants()
        {
            try
            {
                _logger.LogInformation("Admin: Getting all participants");

                var result = await _userService.GetAllParticipantsAsync();

                return Ok(ResponseHelper.SuccessResponse(result, "All participants retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all participants");
                return BadRequest(ResponseHelper.ErrorResponse("Error retrieving participants", ex.Message, 400));
            }
        }
    }
}