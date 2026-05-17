// EMS.API\Controllers\AuthController.cs
using System;
using System.Linq;
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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
 
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
 
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation($"Login attempt for {request.Email}");
                
                if (!ModelState.IsValid)
                    return BadRequest(ResponseHelper.ErrorResponse("Invalid request", "Model validation failed", 400));
 
                var token = await _authService.LoginAsync(request.Email, request.Password);
 
                return Ok(ResponseHelper.SuccessResponse(
                    new { accessToken = token, tokenType = "Bearer", expiresIn = 3600 },
                    "Login successful", 200));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Login failed for {request.Email}: {ex.Message}");
                return Unauthorized(ResponseHelper.UnauthorizedResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                return BadRequest(ResponseHelper.ErrorResponse("Login failed", ex.Message, 400));
            }
        }
 
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation($"Registration attempt for {request.Email}");
 
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ResponseHelper.ErrorResponse("Validation failed", errors, 400));
                }

                var result = await _authService.RegisterAsync(request.Email, request.UserName, request.Password);
                if (!result)
                {
                    _logger.LogWarning($"Registration failed for {request.Email}");
                    return BadRequest(ResponseHelper.ErrorResponse("Registration failed", "Unable to create user", 400));
                }

                return Ok(ResponseHelper.SuccessResponse(
                    new { email = request.Email, userName = request.UserName },
                    "Registration successful", 201));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Registration validation failed for {request.Email}: {ex.Message}");
                // Split error messages by semicolon and return as array
                var errorList = ex.Message.Split(';').Select(e => e.Trim()).ToList();
                return BadRequest(ResponseHelper.ErrorResponse("Validation failed", errorList, 400));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Registration failed for {request.Email}: {ex.Message}");
                return BadRequest(ResponseHelper.ErrorResponse("Registration failed", new List<string> { ex.Message }, 400));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration error");
                return BadRequest(ResponseHelper.ErrorResponse("Registration error", ex.Message, 400));
            }
        }
 
        [HttpPost("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var newToken = await _authService.RefreshTokenAsync(request.OldToken);
 
                return Ok(ResponseHelper.SuccessResponse(
                    new { accessToken = newToken, tokenType = "Bearer" },
                    "Token refreshed", 200));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ResponseHelper.UnauthorizedResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh error");
                return BadRequest(ResponseHelper.ErrorResponse("Token refresh failed", ex.Message, 400));
            }
        }
    }
}