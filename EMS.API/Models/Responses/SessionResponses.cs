// EMS.API\Models\Responses\SessionResponses.cs

using System;
 
namespace EMS.API.Models.Responses
{
    public class SessionResponse
    {
        public Guid SessionId { get; set; }
        public Guid EventId { get; set; }
        public string SessionTitle { get; set; }
        public string Description { get; set; }
        public DateTime SessionStart { get; set; }
        public DateTime SessionEnd { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public Guid? SpeakerId { get; set; }
        public string SpeakerName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
 
    public class SessionListResponse
    {
        public Guid SessionId { get; set; }
        public string SessionTitle { get; set; }
        public DateTime SessionStart { get; set; }
        public DateTime SessionEnd { get; set; }
        public string Location { get; set; }
        public string SpeakerName { get; set; }
    }
}