// EMS.DAL/Repository/ParticipantEventRepository.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.DAL.Data;
using EMS.DAL.Models;
using EMS.DAL.Models.DTOs;

namespace EMS.DAL.Repository
{
    // ─────────────────────────────────────────────────────────────────────────────
    // Interface
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Contract for participant event registration and attendance operations.
    /// </summary>
    public interface IParticipantEventRepository : IRepository<ParticipantEventDetails>
    {
        /// <summary>Register a participant for an event. Throws if already registered.</summary>
        Task<ParticipantEventDetails> RegisterParticipantAsync(string emailId, Guid eventId);

        /// <summary>Return true if the participant is already registered for the event.</summary>
        Task<bool> IsRegisteredAsync(string emailId, Guid eventId);

        /// <summary>Return all event registrations for a participant, newest first.</summary>
        Task<IEnumerable<ParticipantEventDetails>> GetRegisteredEventsAsync(string emailId);

        /// <summary>Return all participants registered for an event.</summary>
        Task<IEnumerable<ParticipantEventDetails>> GetEventParticipantsAsync(Guid eventId);

        /// <summary>Mark a registration as attended.</summary>
        Task<bool> MarkAttendanceAsync(Guid registrationId);

        /// <summary>Return all events a participant has attended.</summary>
        Task<IEnumerable<ParticipantEventDetails>> GetAttendedEventsAsync(string emailId);

        /// <summary>Submit a rating (1–5) and written feedback for a registration.</summary>
        Task<bool> SubmitFeedbackAsync(Guid registrationId, int rating, string feedback);

        /// <summary>
        /// Return attendance statistics for an event.
        /// FIX: was returning <c>dynamic</c> which prevents typed deserialization in the
        /// Angular service layer.  Now returns a concrete <see cref="AttendanceStatsDto"/>.
        /// </summary>
        Task<AttendanceStatsDto> GetAttendanceStatsAsync(Guid eventId);

        /// <summary>Return the total number of events a participant has registered for.</summary>
        Task<int> GetParticipantEventCountAsync(string emailId);

        /// <summary>Remove a participant's registration from an event.</summary>
        Task<bool> UnregisterAsync(string emailId, Guid eventId);

        /// <summary>Return the registration record for a given participant + event pair.</summary>
        Task<ParticipantEventDetails?> GetByEmailAndEventIdAsync(string emailId, Guid eventId);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Implementation
    // ─────────────────────────────────────────────────────────────────────────────

    public class ParticipantEventRepository : Repository<ParticipantEventDetails>, IParticipantEventRepository
    {
        public ParticipantEventRepository(EMSContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<ParticipantEventDetails> RegisterParticipantAsync(string emailId, Guid eventId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId) || eventId == Guid.Empty)
                    throw new ArgumentException("Email ID and Event ID are required.");

                var existing = await _dbSet
                    .FirstOrDefaultAsync(p => p.ParticipantEmailId == emailId && p.EventId == eventId);

                if (existing != null)
                    throw new InvalidOperationException("Participant is already registered for this event.");

                var registration = new ParticipantEventDetails
                {
                    Id                   = Guid.NewGuid(),
                    ParticipantEmailId   = emailId,
                    EventId              = eventId,
                    IsAttended           = false,
                    RegistrationDate     = DateTime.UtcNow,
                    RegistrationStatus   = "Registered"
                };

                await _dbSet.AddAsync(registration);
                await SaveChangesAsync();
                return registration;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error registering participant: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsRegisteredAsync(string emailId, Guid eventId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId) || eventId == Guid.Empty)
                    throw new ArgumentException("Email ID and Event ID are required.");

                return await _dbSet.AnyAsync(p => p.ParticipantEmailId == emailId && p.EventId == eventId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking registration: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ParticipantEventDetails>> GetRegisteredEventsAsync(string emailId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId))
                    throw new ArgumentException("Email ID is required.", nameof(emailId));

                return await _dbSet
                    .Include(p => p.Event)
                    .Where(p => p.ParticipantEmailId == emailId)
                    .OrderByDescending(p => p.RegistrationDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving registered events: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ParticipantEventDetails>> GetEventParticipantsAsync(Guid eventId)
        {
            try
            {
                if (eventId == Guid.Empty)
                    throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

                return await _dbSet
                    .Include(p => p.User)
                    .Where(p => p.EventId == eventId)
                    .OrderBy(p => p.ParticipantEmailId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving event participants: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> MarkAttendanceAsync(Guid registrationId)
        {
            try
            {
                if (registrationId == Guid.Empty)
                    throw new ArgumentException("Registration ID cannot be empty.", nameof(registrationId));

                var reg = await _dbSet.FirstOrDefaultAsync(p => p.Id == registrationId);
                if (reg == null) return false;

                reg.IsAttended         = true;
                reg.AttendanceDate     = DateTime.UtcNow;
                reg.RegistrationStatus = "Attended";

                _dbSet.Update(reg);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error marking attendance: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ParticipantEventDetails>> GetAttendedEventsAsync(string emailId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId))
                    throw new ArgumentException("Email ID is required.", nameof(emailId));

                return await _dbSet
                    .Include(p => p.Event)
                    .Where(p => p.ParticipantEmailId == emailId && p.IsAttended)
                    .OrderByDescending(p => p.AttendanceDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving attended events: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SubmitFeedbackAsync(Guid registrationId, int rating, string feedback)
        {
            try
            {
                if (registrationId == Guid.Empty)
                    throw new ArgumentException("Registration ID cannot be empty.", nameof(registrationId));

                if (rating < 1 || rating > 5)
                    throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

                var reg = await _dbSet.FirstOrDefaultAsync(p => p.Id == registrationId);
                if (reg == null) return false;

                reg.Rating   = rating;
                reg.Feedback = feedback?.Trim();
                // FIX: the original method never updated a LastModified timestamp.
                // ParticipantEventDetails does not have LastModifiedDate, so we update
                // RegistrationStatus to "Feedback Submitted" to surface the change.
                reg.RegistrationStatus = reg.RegistrationStatus == "Attended"
                    ? "Feedback Submitted"
                    : reg.RegistrationStatus;

                _dbSet.Update(reg);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error submitting feedback: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<AttendanceStatsDto> GetAttendanceStatsAsync(Guid eventId)
        {
            try
            {
                if (eventId == Guid.Empty)
                    throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

                var registrations = await _dbSet
                    .Where(p => p.EventId == eventId)
                    .ToListAsync();

                var totalRegistered = registrations.Count;
                var totalAttended   = registrations.Count(p => p.IsAttended);
                var totalNoShow     = totalRegistered - totalAttended;
                var averageRating   = registrations
                    .Where(p => p.Rating.HasValue)
                    .Select(p => (double)p.Rating!.Value)
                    .DefaultIfEmpty(0)
                    .Average();

                return new AttendanceStatsDto
                {
                    EventId              = eventId,
                    TotalRegistered      = totalRegistered,
                    TotalAttended        = totalAttended,
                    TotalNoShow          = totalNoShow,
                    AttendancePercentage = totalRegistered > 0
                        ? Math.Round(totalAttended * 100.0 / totalRegistered, 2)
                        : 0,
                    AverageRating        = Math.Round(averageRating, 2)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving attendance stats: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetParticipantEventCountAsync(string emailId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId))
                    throw new ArgumentException("Email ID is required.", nameof(emailId));

                return await _dbSet.CountAsync(p => p.ParticipantEmailId == emailId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving participant event count: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UnregisterAsync(string emailId, Guid eventId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId) || eventId == Guid.Empty)
                    throw new ArgumentException("Email ID and Event ID are required.");

                var reg = await _dbSet
                    .FirstOrDefaultAsync(p => p.ParticipantEmailId == emailId && p.EventId == eventId);

                if (reg == null) return false;

                _dbSet.Remove(reg);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error unregistering participant: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ParticipantEventDetails?> GetByEmailAndEventIdAsync(string emailId, Guid eventId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId) || eventId == Guid.Empty)
                    throw new ArgumentException("Email ID and Event ID are required.");

                return await _dbSet
                    .Include(p => p.Event)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.ParticipantEmailId == emailId && p.EventId == eventId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving registration: {ex.Message}", ex);
            }
        }
    }
}