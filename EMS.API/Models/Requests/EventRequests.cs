// EMS.API\Models\Requests\EventRequests.cs

using System;
using System.ComponentModel.DataAnnotations;
 
namespace EMS.API.Models.Requests
{
    public class CreateEventRequest
    {
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(200)]
        public string EventName { get; set; }
 
        [Required(ErrorMessage = "Category is required")]
        [StringLength(50)]
        public string EventCategory { get; set; }
 
        [Required(ErrorMessage = "Event date is required")]
        public DateTime EventDate { get; set; }
 
        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; }
 
        [Required(ErrorMessage = "Location is required")]
        [StringLength(200)]
        public string Location { get; set; }
 
        public int? MaxParticipants { get; set; }
    }
 
    public class UpdateEventRequest
    {
        [Required]
        public Guid EventId { get; set; }
 
        [Required(ErrorMessage = "Event name is required")]
        [StringLength(200)]
        public string EventName { get; set; }
 
        [Required(ErrorMessage = "Category is required")]
        [StringLength(50)]
        public string EventCategory { get; set; }
 
        [Required(ErrorMessage = "Event date is required")]
        public DateTime EventDate { get; set; }
 
        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000)]
        public string Description { get; set; }
 
        [Required(ErrorMessage = "Location is required")]
        [StringLength(200)]
        public string Location { get; set; }
 
        public int? MaxParticipants { get; set; }
    }
}