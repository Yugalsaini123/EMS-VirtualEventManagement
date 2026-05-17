//EMS.Services/Implementations/EventService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.DAL.Models;
using EMS.DAL.Repository;
using EMS.Services.Interfaces;
using EMS.Services.Helpers;

namespace EMS.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IParticipantEventRepository _participantEventRepository;
        private readonly ISessionRepository _sessionRepository;

        public EventService(
            IEventRepository eventRepository,
            IParticipantEventRepository participantEventRepository,
            ISessionRepository sessionRepository)
        {
            _eventRepository = eventRepository;
            _participantEventRepository = participantEventRepository;
            _sessionRepository = sessionRepository;
        }


        private async Task<bool> ShouldEventBeInactiveAsync(Guid eventId, DateTime eventDate)
        {
            // If event date is in the future, it's Active (not inactive)
            if (eventDate > DateTime.UtcNow)
                return false;

            // Event date is in the past, now check last session end time
            var sessions = await _sessionRepository.GetSessionsByEventAsync(eventId);
            
            if (!sessions.Any())
            {
                // No sessions and event date passed, so mark Inactive
                return true;
            }

            // Find the latest session end time
            var lastSessionEnd = sessions.Max(s => s.SessionEnd);
            
            // If last session is still in the future, keep Active (return false for inactive)
            if (lastSessionEnd > DateTime.UtcNow)
                return false;

            // Both event date and last session are in the past, mark as Inactive
            return true;
        }

        private bool ShouldEventBeInactiveSimple(DateTime eventDate)
        {
            if (eventDate > DateTime.UtcNow)
                return false;  // Active
            return true;  // Inactive
        }

        public async Task<PaginatedResponse<EventListDto>> GetAllEventsAsync(int pageNumber, int pageSize, string? search = null, string? sortBy = null, string? sortOrder = null)
        {
            var events = (await _eventRepository.GetEventsWithParticipantCountAsync()).ToList();

            // Mark events as Inactive if event date AND last session date have passed
            foreach (var evt in events)
            {
                var shouldBeInactive = await ShouldEventBeInactiveAsync(evt.EventId, evt.EventDate);
                
                // Update database if status needs to change to Inactive
                if (shouldBeInactive && evt.Status != "Inactive")
                {
                    var eventEntity = await _eventRepository.GetByIdAsync(evt.EventId);
                    if (eventEntity != null && eventEntity.Status != "Inactive")
                    {
                        eventEntity.Status = "Inactive";
                        await _eventRepository.UpdateAsync(eventEntity);
                    }
                }
                
                // Set the status for response
                evt.Status = shouldBeInactive ? "Inactive" : "Active";
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                events = events.Where(e =>
                    (e.EventName ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (e.EventCategory ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (e.Location ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            sortBy = sortBy?.ToLowerInvariant();
            sortOrder = sortOrder?.ToLowerInvariant() == "desc" ? "desc" : "asc";

            events = sortBy switch
            {
                "name" => sortOrder == "desc"
                    ? events.OrderByDescending(e => e.EventName).ToList()
                    : events.OrderBy(e => e.EventName).ToList(),
                "category" => sortOrder == "desc"
                    ? events.OrderByDescending(e => e.EventCategory).ToList()
                    : events.OrderBy(e => e.EventCategory).ToList(),
                "participants" => sortOrder == "desc"
                    ? events.OrderByDescending(e => e.ParticipantCount).ToList()
                    : events.OrderBy(e => e.ParticipantCount).ToList(),
                _ => sortOrder == "desc"
                    ? events.OrderByDescending(e => e.EventDate).ToList()
                    : events.OrderBy(e => e.EventDate).ToList()
            };

            var totalCount = events.Count;
            var pagedEvents = events
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return ResponseHelper.PaginatedResponse(pagedEvents, totalCount, pageNumber, pageSize);
        }

        public async Task<dynamic> GetEventByIdAsync(Guid eventId)
        {
            var eventDetails = await _eventRepository.GetEventWithDetailsAsync(eventId);
            if (eventDetails == null)
                throw new KeyNotFoundException("Event not found");

            var shouldBeInactive = await ShouldEventBeInactiveAsync(eventId, eventDetails.EventDate);
            var status = shouldBeInactive ? "Inactive" : "Active";
            
            // Update database if status needs to change to Inactive
            if (shouldBeInactive && eventDetails.Status != "Inactive")
            {
                var eventEntity = await _eventRepository.GetByIdAsync(eventId);
                if (eventEntity != null && eventEntity.Status != "Inactive")
                {
                    eventEntity.Status = "Inactive";
                    await _eventRepository.UpdateAsync(eventEntity);
                }
            }

            return new
            {
                eventDetails.EventId,
                eventDetails.EventName,
                eventDetails.EventCategory,
                eventDetails.EventDate,
                eventDetails.Description,
                Status = status,
                eventDetails.Location,
                eventDetails.MaxParticipants,
                ParticipantCount = eventDetails.ParticipantRegistrations?.Count ?? 0
            };
        }

        public async Task<dynamic> CreateEventAsync(string eventName, string category, DateTime eventDate,
            string description, string location, int? maxParticipants)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(eventName) || eventName.Length < 3)
                throw new ArgumentException("Event name must be at least 3 characters");

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category is required");

            if (string.IsNullOrWhiteSpace(description) || description.Length < 10)
                throw new ArgumentException("Description must be at least 10 characters");

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location is required");

            if (maxParticipants.HasValue && maxParticipants.Value < 1)
                throw new ArgumentException("Max participants must be at least 1");

            // Event date must be in the future
            if (eventDate <= DateTime.UtcNow)
                throw new ArgumentException("Event date must be in the future");

            var newEvent = new EventDetails
            {
                EventId = Guid.NewGuid(),
                EventName = eventName,
                EventCategory = category,
                EventDate = eventDate,
                Description = description,
                Location = location,
                MaxParticipants = maxParticipants,
                Status = ShouldEventBeInactiveSimple(eventDate) ? "Inactive" : "Active",
                CreatedDate = DateTime.UtcNow
            };

            await _eventRepository.AddAsync(newEvent);

            return new
            {
                newEvent.EventId,
                newEvent.EventName,
                newEvent.EventCategory,
                newEvent.EventDate,
                Status = ShouldEventBeInactiveSimple(newEvent.EventDate) ? "Inactive" : "Active"
            };
        }

        public async Task<dynamic> UpdateEventAsync(Guid eventId, string eventName, string category,
            DateTime eventDate, string description, string location, int? maxParticipants)
        {
            var eventDetails = await _eventRepository.GetByIdAsync(eventId);
            if (eventDetails == null)
                throw new KeyNotFoundException("Event not found");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(eventName) || eventName.Length < 3)
                throw new ArgumentException("Event name must be at least 3 characters");

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category is required");

            if (string.IsNullOrWhiteSpace(description) || description.Length < 10)
                throw new ArgumentException("Description must be at least 10 characters");

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location is required");

            if (maxParticipants.HasValue && maxParticipants.Value < 1)
                throw new ArgumentException("Max participants must be at least 1");

            if (maxParticipants.HasValue)
            {
                var currentRegistrations = (await _participantEventRepository.GetEventParticipantsAsync(eventId)).Count();
                if (currentRegistrations > maxParticipants.Value)
                    throw new InvalidOperationException($"Max participants cannot be less than current registration count ({currentRegistrations})");
            }

            // Event date must be in the future
            if (eventDate <= DateTime.UtcNow)
                throw new ArgumentException("Event date must be in the future");

            eventDetails.EventName = eventName;
            eventDetails.EventCategory = category;
            eventDetails.EventDate = eventDate;
            eventDetails.Description = description;
            eventDetails.Location = location;
            eventDetails.MaxParticipants = maxParticipants;
            eventDetails.Status = ShouldEventBeInactiveSimple(eventDate) ? "Inactive" : "Active";
            eventDetails.LastModifiedDate = DateTime.UtcNow;

            await _eventRepository.UpdateAsync(eventDetails);

            var shouldBeInactive = await ShouldEventBeInactiveAsync(eventId, eventDate);
            var status = shouldBeInactive ? "Inactive" : "Active";
            return new
            {
                eventDetails.EventId,
                eventDetails.EventName,
                eventDetails.EventCategory,
                eventDetails.EventDate,
                Status = status
            };
        }

        public async Task<bool> DeleteEventAsync(Guid eventId)
        {
            var eventDetails = await _eventRepository.GetEventWithDetailsAsync(eventId);
            if (eventDetails == null)
                throw new KeyNotFoundException("Event not found");

            // Check if event has any registered participants
            var registrations = await _participantEventRepository.GetEventParticipantsAsync(eventId);
            if (registrations.Any())
                throw new InvalidOperationException("Cannot delete event with registered participants");

            await _eventRepository.RemoveAsync(eventDetails);
            return true;
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            var categories = await _eventRepository.GetCategoriesAsync();
            return categories.ToList();
        }

        public async Task<List<dynamic>> GetEventsByCategoryAsync(string category)
        {
            var events = await _eventRepository.GetByCategoryAsync(category);

            return events.Select(e => (dynamic)new
            {
                e.EventId,
                e.EventName,
                e.EventCategory,
                e.EventDate,
                Status = ShouldEventBeInactiveSimple(e.EventDate) ? "Inactive" : "Active"
            }).ToList();
        }

        public async Task<List<dynamic>> GetUpcomingEventsAsync()
        {
            var events = await _eventRepository.GetUpcomingEventsAsync();

            return events.Select(e => (dynamic)new
            {
                e.EventId,
                e.EventName,
                e.EventCategory,
                e.EventDate,
                Status = ShouldEventBeInactiveSimple(e.EventDate) ? "Inactive" : "Active"
            }).ToList();
        }

        public async Task<List<dynamic>> GetAllEventsAdminAsync()
        {
            var events = await _eventRepository.GetAllEventsWithCountAsync();

            return events.Select(e => (dynamic)new
            {
                e.EventId,
                e.EventName,
                e.EventCategory,
                e.EventDate,
                e.Status,
                e.Location,
                e.MaxParticipants,
                e.ParticipantCount,
                e.SessionCount,
                e.CreatedDate,
                e.LastModifiedDate
            }).ToList();
        }

        public async Task<bool> ToggleEventStatusAsync(Guid eventId)
        {
            return await _eventRepository.ToggleEventStatusAsync(eventId);
        }
    }
}
