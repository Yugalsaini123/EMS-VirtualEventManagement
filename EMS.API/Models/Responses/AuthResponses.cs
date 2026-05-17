// EMS.API\Models\Responses\AuthResponses.cs

using System;
 
namespace EMS.API.Models.Responses
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresIn { get; set; } = 3600;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    }
 
    public class UserResponse
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
 
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TokenResponse Token { get; set; }
        public UserResponse User { get; set; }
    }
}