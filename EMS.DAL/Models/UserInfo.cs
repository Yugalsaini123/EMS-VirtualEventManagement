// EMS.DAL/Models/UserInfo.cs  (unchanged from original)
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.DAL.Models
{
    [Table("UserInfo")]
    public class UserInfo
    {
        [Key]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string EmailId { get; set; } = string.Empty;

        [Required(ErrorMessage = "User Name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "User Name must be between 1 and 50 characters")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(200)]   
        public string Password { get; set; } = string.Empty;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<ParticipantEventDetails> ParticipantEvents { get; set; } = new List<ParticipantEventDetails>();
    }
}