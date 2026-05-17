// EMS.API\Models\Requests\ParticipantRequests.cs

using System;
using System.ComponentModel.DataAnnotations;
 
namespace EMS.API.Models.Requests
{
    public class RegisterForEventRequest
    {
        [Required]
        public Guid EventId { get; set; }
    }
 
    public class MarkAttendanceRequest
    {
        [Required]
        public Guid RegistrationId { get; set; }
    }
 
    public class SubmitFeedbackRequest
    {
        [Required]
        public Guid RegistrationId { get; set; }
 
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
 
        [StringLength(500)]
        public string Feedback { get; set; }
    }
 
    public class UnregisterRequest
    {
        [Required]
        public Guid EventId { get; set; }
    }
}