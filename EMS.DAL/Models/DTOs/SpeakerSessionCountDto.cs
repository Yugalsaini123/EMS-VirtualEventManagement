// EMS.DAL/Models/DTOs/SpeakerSessionCountDto.cs
using System;

namespace EMS.DAL.Models.DTOs
{
    /// <summary>
    /// Projection of a speaker record combined with the number of sessions
    /// they are assigned to.  Replaces the <c>dynamic</c> return type in
    /// <c>SpeakerRepository.GetSpeakersWithSessionCountAsync</c>.
    /// </summary>
    public class SpeakerSessionCountDto
    {
        public Guid   SpeakerId    { get; set; }
        public string SpeakerName  { get; set; } = string.Empty;
        public string Email        { get; set; } = string.Empty;
        public string Designation  { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public int    SessionCount { get; set; }
    }
}