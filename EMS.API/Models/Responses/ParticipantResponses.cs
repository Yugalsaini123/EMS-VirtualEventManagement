// EMS.API\Models\Responses\ParticipantResponses.cs

using System;
 
namespace EMS.API.Models.Responses
{
    public class ParticipantEventResponse
    {
        public Guid RegistrationId { get; set; }
        public Guid EventId { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public string ParticipantEmail { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsAttended { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public int? Rating { get; set; }
        public string Feedback { get; set; }
    }
 
    public class ParticipantListResponse
    {
        public Guid RegistrationId { get; set; }
        public string ParticipantEmail { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsAttended { get; set; }
        public int? Rating { get; set; }
    }
}