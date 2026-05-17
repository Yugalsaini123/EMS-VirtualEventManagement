//EMS.API/Models/Requests/UpdateUserRequest.cs
using System.ComponentModel.DataAnnotations;

namespace EMS.API.Models.Requests
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 100 characters")]
        public string UserName { get; set; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters")]
        public string Password { get; set; }
    }
}
