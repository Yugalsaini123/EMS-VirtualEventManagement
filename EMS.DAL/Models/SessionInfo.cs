// EMS.DAL/Models/SessionInfo.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.DAL.Models
{
    /// <summary>
    /// Entity representing a session within an event.
    /// </summary>
    [Table("SessionInfo")]
    public class SessionInfo
    {
        /// <summary>Primary Key (GUID).</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SessionId { get; set; }

        /// <summary>
        /// FK → EventDetails. Required – every session belongs to an event.
        /// </summary>
        [Required(ErrorMessage = "Event ID is required")]
        public Guid EventId { get; set; }

        /// <summary>Session title (1–100 characters).</summary>
        [Required(ErrorMessage = "Session Title is required")]
        [StringLength(100, MinimumLength = 1,
            ErrorMessage = "Session Title must be between 1 and 100 characters")]
        public string SessionTitle { get; set; } = string.Empty;

        /// <summary>
        /// FK → SpeakersDetails. Optional – a session may not have a speaker yet.
        /// </summary>
        public Guid? SpeakerId { get; set; }

        /// <summary>Optional session description (max 500 characters).</summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        /// <summary>Session start time. Must be before <see cref="SessionEnd"/>.</summary>
        [Required(ErrorMessage = "Session Start time is required")]
        public DateTime SessionStart { get; set; }

        /// <summary>Session end time. Must be after <see cref="SessionStart"/>.</summary>
        [Required(ErrorMessage = "Session End time is required")]
        public DateTime SessionEnd { get; set; }

        /// <summary>Meeting link / URL for virtual or hybrid sessions.</summary>
        [Url(ErrorMessage = "Invalid URL format")]
        public string? SessionUrl { get; set; }

        /// <summary>Maximum attendee capacity for this session.</summary>
        public int? Capacity { get; set; }

        /// <summary>Physical or virtual location (max 200 characters).</summary>
        [StringLength(200)]
        public string? Location { get; set; }

        /// <summary>Session lifecycle status: Scheduled | Completed | Cancelled.</summary>
        [StringLength(20)]
        public string Status { get; set; } = "Scheduled";

        /// <summary>Record creation timestamp (set by DB default / C# initialiser).</summary>
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>Last modification timestamp.</summary>
        public DateTime? LastModifiedDate { get; set; }

        // ── Navigation properties ───────────────────────────────────────────────

        /// <summary>
        /// Parent event.
        /// FIX: was named "EventDetails" which collided with the EMS.DAL.Models.EventDetails
        /// class name and the EMSContext.EventDetails DbSet, causing EF mapping ambiguity
        /// and confusing IntelliSense.  Renamed to "Event".
        /// </summary>
        [ForeignKey(nameof(EventId))]
        public virtual EventDetails? Event { get; set; }

        /// <summary>Assigned speaker (nullable).</summary>
        [ForeignKey(nameof(SpeakerId))]
        public virtual SpeakersDetails? Speaker { get; set; }
    }
}