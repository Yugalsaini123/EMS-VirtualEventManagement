// EMS.DAL/Models/SpeakersDetails.cs  (unchanged from original)
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.DAL.Models
{
    [Table("SpeakersDetails")]
    public class SpeakersDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SpeakerId { get; set; }

        [Required(ErrorMessage = "Speaker Name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Speaker Name must be between 1 and 50 characters")]
        public string SpeakerName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Designation { get; set; }

        [StringLength(100)]
        public string? Organization { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? PhoneNumber { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? LinkedInUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<SessionInfo> Sessions { get; set; } = new List<SessionInfo>();
    }
}