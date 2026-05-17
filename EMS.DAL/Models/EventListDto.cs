using System;

namespace EMS.DAL.Models
{
    /// <summary>
    /// Lightweight projection used in the public / participant event listing.
    /// Only Active events are returned for this DTO.
    /// </summary>
    public class EventListDto
    {
        public Guid    EventId          { get; set; }
        public string  EventName        { get; set; } = string.Empty;
        public string  EventCategory    { get; set; } = string.Empty;
        public DateTime EventDate       { get; set; }
        public string  Status           { get; set; } = string.Empty;
        public string? Location         { get; set; }
        public int?    MaxParticipants  { get; set; }
        public int     ParticipantCount { get; set; }
        public int     SessionCount     { get; set; }
    }

    /// <summary>
    /// Admin-facing event listing DTO – identical fields but includes ALL statuses
    /// (Active AND Inactive) so the admin dashboard shows the full picture.
    /// Having a separate DTO keeps the query intent explicit and avoids accidental
    /// exposure of inactive events on the public-facing API endpoints.
    /// </summary>
    public class EventListAdminDto : EventListDto
    {
        public DateTime CreatedDate          { get; set; }
        public DateTime? LastModifiedDate    { get; set; }
    }
}