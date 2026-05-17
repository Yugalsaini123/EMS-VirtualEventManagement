// EMS.API\Models\Requests\SpeakerRequests.cs

using System;
using System.ComponentModel.DataAnnotations;
 
namespace EMS.API.Models.Requests
{
    public class CreateSpeakerRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100)]
        public string SpeakerName { get; set; }
 
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
 
        [StringLength(100)]
        public string Designation { get; set; }
 
        [StringLength(100)]
        public string Organization { get; set; }
 
        [StringLength(500)]
        public string Bio { get; set; }
 
        [Phone]
        public string PhoneNumber { get; set; }
 
        [Url]
        public string? LinkedInUrl { get; set; }
    }
 
    public class UpdateSpeakerRequest
    {
        [Required]
        public Guid SpeakerId { get; set; }
 
        [Required]
        public string SpeakerName { get; set; }
 
        [Required]
        [EmailAddress]
        public string Email { get; set; }
 
        public string Designation { get; set; }
 
        public string Organization { get; set; }
 
        public string Bio { get; set; }
 
        [Phone]
        public string PhoneNumber { get; set; }
 
        [Url]
        public string? LinkedInUrl { get; set; }
    }
}
