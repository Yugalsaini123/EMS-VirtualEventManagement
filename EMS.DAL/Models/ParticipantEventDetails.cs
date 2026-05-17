// EMS.DAL/Models/ParticipantEventDetails.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.DAL.Models
{
    /// <summary>
    /// Join entity tracking which participants have registered for which events,
    /// their attendance, feedback, and registration status.
    /// </summary>
    [Table("ParticipantEventDetails")]
    public class ParticipantEventDetails
    {
        /// <summary>Registration record PK (GUID).</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// FK → UserInfo.EmailId.
        /// Required – every registration must be linked to a user account.
        /// </summary>
        [Required(ErrorMessage = "Participant Email ID is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string ParticipantEmailId { get; set; } = string.Empty;

        /// <summary>
        /// FK → EventDetails.EventId.
        /// Required – every registration must be for a specific event.
        /// </summary>
        [Required(ErrorMessage = "Event ID is required")]
        public Guid EventId { get; set; }

        /// <summary>Whether the participant actually attended the event.</summary>
        public bool IsAttended { get; set; } = false;

        /// <summary>Timestamp when the participant registered.</summary>
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        /// <summary>Timestamp when attendance was recorded (null until marked).</summary>
        public DateTime? AttendanceDate { get; set; }

        /// <summary>Optional admin / internal notes about this registration.</summary>
        [StringLength(500)]
        public string? Notes { get; set; }

        /// <summary>Participant star rating (1–5). Null until feedback is submitted.</summary>
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int? Rating { get; set; }

        /// <summary>Participant written feedback. Null until feedback is submitted.</summary>
        [StringLength(1000)]
        public string? Feedback { get; set; }

        /// <summary>
        /// Lifecycle status of this registration:
        /// Registered | Attended | Cancelled | No-Show.
        /// </summary>
        [StringLength(20)]
        public string RegistrationStatus { get; set; } = "Registered";

        // ── Navigation properties ───────────────────────────────────────────────

        /// <summary>Related event record.</summary>
        [ForeignKey(nameof(EventId))]
        public virtual EventDetails? Event { get; set; }

        /// <summary>Related user / participant record.</summary>
        [ForeignKey(nameof(ParticipantEmailId))]
        public virtual UserInfo? User { get; set; }
    }
}