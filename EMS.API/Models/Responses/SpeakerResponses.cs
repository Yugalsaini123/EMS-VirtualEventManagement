// EMS.API\Models\Responses\SpeakerResponses.cs

using System;
 
namespace EMS.API.Models.Responses
{
    public class SpeakerResponse
    {
        public Guid SpeakerId { get; set; }
        public string SpeakerName { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
        public string Organization { get; set; }
        public string Bio { get; set; }
        public string PhoneNumber { get; set; }
        public string? LinkedInUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
 
    public class SpeakerListResponse
    {
        public Guid SpeakerId { get; set; }
        public string SpeakerName { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
        public string Organization { get; set; }
        public bool IsActive { get; set; }
    }
}