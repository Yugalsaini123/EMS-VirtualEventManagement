// EMS.API/Models/Requests/ChangePasswordRequest.cs
using System.ComponentModel.DataAnnotations;

namespace EMS.API.Models.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be 8-20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$",
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}