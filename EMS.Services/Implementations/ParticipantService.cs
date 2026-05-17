// EMS.Services\Implementations\ParticipantService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.DAL.Models;
using EMS.DAL.Repository;
using EMS.Services.Interfaces;

namespace EMS.Services.Implementations
{
    public class ParticipantService : IParticipantService
    {
        private readonly IParticipantEventRepository _participantEventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;

        public ParticipantService(
            IParticipantEventRepository participantEventRepository,
            IUserRepository userRepository,
            IEventRepository eventRepository)
        {
            _participantEventRepository = participantEventRepository;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }

        public async Task<bool> RegisterForEventAsync(string email, Guid eventId)
        {
            // Validate user exists
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Validate event exists and is Active
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException("Event not found");

            // Check if event is Inactive
            if (eventEntity.Status == "Inactive")
                throw new InvalidOperationException("Cannot register for inactive events");

            // Check if already registered
            var existingRegistration = await _participantEventRepository.GetByEmailAndEventIdAsync(email, eventId);
            if (existingRegistration != null)
                throw new InvalidOperationException("Already registered for this event");

            // Check event capacity
            if (eventEntity.MaxParticipants.HasValue)
            {
                var currentParticipants = await _participantEventRepository.GetEventParticipantsAsync(eventId);
                if (currentParticipants.Count() >= eventEntity.MaxParticipants.Value)
                    throw new InvalidOperationException("Event has reached maximum capacity");
            }

            // Create registration
            var registration = new ParticipantEventDetails
            {
                Id = Guid.NewGuid(),
                ParticipantEmailId = email,
                EventId = eventId,
                RegistrationDate = DateTime.UtcNow,
                IsAttended = false,
                RegistrationStatus = "Registered"
            };

            await _participantEventRepository.AddAsync(registration);
            return true;
        }

        public async Task<List<dynamic>> GetRegisteredEventsAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            var registrations = await _participantEventRepository.GetRegisteredEventsAsync(email);

            var result = new List<dynamic>();
            var now = DateTime.UtcNow;

            foreach (var r in registrations)
            {
                // Use event status from database (which is either Active or Inactive)
                string status = r.Event?.Status ?? "Active";

                result.Add(new
                {
                    RegistrationId = r.Id,
                    r.EventId,
                    EventName = r.Event?.EventName ?? "Unknown",
                    EventDate = r.Event?.EventDate,
                    r.RegistrationDate,
                    r.IsAttended,
                    r.Rating,
                    r.Feedback,
                    Status = status
                });
            }

            return result;
        }

        public async Task<bool> UnregisterFromEventAsync(string email, Guid eventId)
        {
            var registration = await _participantEventRepository.GetByEmailAndEventIdAsync(email, eventId);
            if (registration == null)
                throw new KeyNotFoundException("Registration not found");

            // Check if event is Inactive (cannot unregister from inactive events)
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException("Event not found");

            if (eventEntity.Status == "Inactive")
                throw new InvalidOperationException("Cannot unregister from inactive events");

            // Cannot unregister if already attended
            if (registration.IsAttended)
                throw new InvalidOperationException("Cannot unregister from attended events");

            await _participantEventRepository.RemoveAsync(registration);
            return true;
        }

        public async Task<bool> MarkAttendanceAsync(Guid registrationId)
        {
            var registration = await _participantEventRepository.GetByIdAsync(registrationId);
            if (registration == null)
                throw new KeyNotFoundException("Registration not found");
        
            var eventEntity = await _eventRepository.GetByIdAsync(registration.EventId);
            if (eventEntity == null)
                throw new KeyNotFoundException("Event not found");
        
            // Update attendance
            registration.IsAttended = true;
            registration.AttendanceDate = DateTime.UtcNow;
            registration.RegistrationStatus = "Attended";
        
            await _participantEventRepository.UpdateAsync(registration);
            return true;
        }

        public async Task<bool> SubmitFeedbackAsync(Guid registrationId, int rating, string feedback)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var registration = await _participantEventRepository.GetByIdAsync(registrationId);
            if (registration == null)
                throw new KeyNotFoundException("Registration not found");

            // Can only submit feedback if attended
            if (!registration.IsAttended)
                throw new InvalidOperationException("Feedback can only be submitted for attended events");

            // Update feedback
            registration.Rating = rating;
            registration.Feedback = feedback;

            await _participantEventRepository.UpdateAsync(registration);
            return true;
        }

        public async Task<List<dynamic>> GetEventParticipantsAsync(Guid eventId)
        {
            var registrations = await _participantEventRepository.GetEventParticipantsAsync(eventId);
            return registrations.Select(r => (dynamic)new
            {
                r.Id,
                r.ParticipantEmailId,
                r.RegistrationDate,
                r.IsAttended,
                r.Rating,
                r.Feedback
            }).ToList();
        }

        public async Task<bool> IsRegisteredAsync(string email, Guid eventId)
        {
            return await _participantEventRepository.IsRegisteredAsync(email, eventId);
        }

        public async Task<dynamic> GetRegistrationByIdAsync(Guid registrationId)
        {
            var registration = await _participantEventRepository.GetByIdAsync(registrationId);
            if (registration == null)
                return null;

            return new
            {
                registration.Id,
                registration.ParticipantEmailId,
                registration.EventId,
                registration.IsAttended,
                registration.RegistrationDate,
                registration.AttendanceDate,
                registration.Rating,
                registration.Feedback
            };
        }

        public async Task<dynamic> GetAttendanceStatsAsync(Guid eventId)
        {
            var stats = await _participantEventRepository.GetAttendanceStatsAsync(eventId);
            
            return new
            {
                stats.EventId,
                stats.TotalRegistered,
                stats.TotalAttended,
                stats.TotalNoShow,
                stats.AttendancePercentage,
                stats.AverageRating
            };
        }
    }
}
