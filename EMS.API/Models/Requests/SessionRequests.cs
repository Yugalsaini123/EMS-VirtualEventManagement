// EMS.API\Models\Requests\SessionRequests.cs

using System;
using System.ComponentModel.DataAnnotations;
 
namespace EMS.API.Models.Requests
{
    public class CreateSessionRequest
    {
        [Required]
        public Guid EventId { get; set; }
 
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200)]
        public string Title { get; set; }
 
        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }
 
        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }
 
        [StringLength(500)]
        public string Description { get; set; }
 
        [StringLength(200)]
        public string Location { get; set; }
 
        public Guid? SpeakerId { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? SessionUrl { get; set; }
    }
 
    public class UpdateSessionRequest
    {
        [Required]
        public Guid SessionId { get; set; }
 
        [Required]
        public string Title { get; set; }
 
        [Required]
        public DateTime StartTime { get; set; }
 
        [Required]
        public DateTime EndTime { get; set; }
 
        public string Description { get; set; }
 
        public string Location { get; set; }
 
        public Guid? SpeakerId { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public string? SessionUrl { get; set; }
    }
 
    public class AssignSpeakerRequest
    {
        [Required]
        public Guid SessionId { get; set; }
 
        [Required]
        public Guid SpeakerId { get; set; }
    }
}