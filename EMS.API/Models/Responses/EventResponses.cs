// EMS.API\Models\Responses\EventResponses.cs

using System;
 
namespace EMS.API.Models.Responses
{
    public class EventResponse
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public string EventCategory { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public int? MaxParticipants { get; set; }
        public int ParticipantCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
 
    public class EventListResponse
    {
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public string EventCategory { get; set; }
        public DateTime EventDate { get; set; }
        public string Status { get; set; }
        public int ParticipantCount { get; set; }
    }
}