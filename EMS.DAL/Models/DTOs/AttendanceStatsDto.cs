// EMS.DAL/Models/DTOs/AttendanceStatsDto.cs
using System;

namespace EMS.DAL.Models.DTOs
{
    /// <summary>
    /// Attendance statistics summary for a single event.
    /// Replaces the <c>dynamic</c> return type that previously existed in
    /// <c>ParticipantEventRepository.GetAttendanceStatsAsync</c>. Strongly-typed
    /// DTOs are required for correct JSON serialisation in the Web API layer and
    /// for typed models in the Angular frontend.
    /// </summary>
    public class AttendanceStatsDto
    {
        public Guid   EventId               { get; set; }
        public int    TotalRegistered       { get; set; }
        public int    TotalAttended         { get; set; }
        public int    TotalNoShow           { get; set; }
        public double AttendancePercentage  { get; set; }
        public double AverageRating         { get; set; }
    }
}