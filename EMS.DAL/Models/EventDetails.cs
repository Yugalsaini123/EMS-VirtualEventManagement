// EMS.DAL/Models/EventDetails.cs  (unchanged from original)
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.DAL.Models
{
    [Table("EventDetails")]
    public class EventDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EventId { get; set; }

        [Required(ErrorMessage = "Event Name is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Event Name must be between 1 and 100 characters")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event Category is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Event Category must be between 1 and 50 characters")]
        public string EventCategory { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event Date is required")]
        public DateTime EventDate { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        [StringLength(200)]
        public string? Location { get; set; }

        public int? MaxParticipants { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedDate { get; set; }

        public virtual ICollection<SessionInfo> Sessions { get; set; } = new List<SessionInfo>();

        public virtual ICollection<ParticipantEventDetails> ParticipantRegistrations { get; set; } = new List<ParticipantEventDetails>();
    }
}