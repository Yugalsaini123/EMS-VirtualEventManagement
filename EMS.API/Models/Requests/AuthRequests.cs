// EMS.API\Models\Requests\AuthRequests.cs
using System.ComponentModel.DataAnnotations;
 
namespace EMS.API.Models.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
 
        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Password must be 4-20 characters")]
        public string Password { get; set; }
    }
 
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
 
        [Required(ErrorMessage = "User name is required")]
        [StringLength(100, MinimumLength = 2)]
        public string UserName { get; set; }
 
        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be 8-20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$",
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    public class RefreshTokenRequest
    {
        [Required]
        public string OldToken { get; set; }
    }
}
 