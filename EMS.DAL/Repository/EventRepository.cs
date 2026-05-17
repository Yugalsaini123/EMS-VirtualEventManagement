// EMS.DAL/Repository/EventRepository.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.DAL.Data;
using EMS.DAL.Models;

namespace EMS.DAL.Repository
{
    // ─────────────────────────────────────────────────────────────────────────────
    // Interface
    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Contract for all event-related data operations.
    /// </summary>
    public interface IEventRepository : IRepository<EventDetails>
    {
        /// <summary>Get a single event with its sessions (+ each session's speaker) and participants.</summary>
        Task<EventDetails?> GetEventWithDetailsAsync(Guid eventId);

        /// <summary>Get Active events in a given category.</summary>
        Task<IEnumerable<EventDetails>> GetByCategoryAsync(string category);

        /// <summary>Get Active events whose date is today or later.</summary>
        Task<IEnumerable<EventDetails>> GetUpcomingEventsAsync();

        /// <summary>Get events whose date is in the past (any status).</summary>
        Task<IEnumerable<EventDetails>> GetPastEventsAsync();

        /// <summary>Get all events with Status == "Active".</summary>
        Task<IEnumerable<EventDetails>> GetActiveEventsAsync();

        /// <summary>Case-insensitive name search over Active events.</summary>
        Task<IEnumerable<EventDetails>> SearchByNameAsync(string name);

        /// <summary>
        /// Public-facing listing: Active events only, with participant and session counts.
        /// </summary>
        Task<IEnumerable<EventListDto>> GetEventsWithParticipantCountAsync();

        /// <summary>
        /// Admin-facing listing: ALL events regardless of status, with participant,
        /// session counts, and audit dates.
        /// FIX: the original method filtered to Active only, so the admin dashboard
        /// could never see or re-activate an Inactive event.
        /// </summary>
        Task<IEnumerable<EventListAdminDto>> GetAllEventsWithCountAsync();

        /// <summary>Toggle event status between Active and Inactive.</summary>
        Task<bool> ToggleEventStatusAsync(Guid eventId);

        /// <summary>Get all distinct event category strings.</summary>
        Task<IEnumerable<string>> GetCategoriesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Implementation
    // ─────────────────────────────────────────────────────────────────────────────

    public class EventRepository : Repository<EventDetails>, IEventRepository
    {
        public EventRepository(EMSContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<EventDetails?> GetEventWithDetailsAsync(Guid eventId)
        {
            try
            {
                if (eventId == Guid.Empty)
                    throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

                return await _dbSet
                    .Include(e => e.Sessions)
                        .ThenInclude(s => s.Speaker)
                    .Include(e => e.ParticipantRegistrations)
                    .FirstOrDefaultAsync(e => e.EventId == eventId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving event with details: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EventDetails>> GetByCategoryAsync(string category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                    throw new ArgumentException("Category cannot be empty.", nameof(category));

                return await _dbSet
                    .Where(e => e.EventCategory == category && e.Status == "Active")
                    .OrderByDescending(e => e.EventDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving events by category: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EventDetails>> GetUpcomingEventsAsync()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                return await _dbSet
                    .Where(e => e.EventDate >= today && e.Status == "Active")
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving upcoming events: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EventDetails>> GetPastEventsAsync()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                return await _dbSet
                    .Where(e => e.EventDate < today)
                    .OrderByDescending(e => e.EventDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving past events: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EventDetails>> GetActiveEventsAsync()
        {
            try
            {
                return await _dbSet
                    .Where(e => e.Status == "Active")
                    .OrderByDescending(e => e.EventDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving active events: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EventDetails>> SearchByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Search term cannot be empty.", nameof(name));

                return await _dbSet
                    .Where(e => e.EventName.Contains(name) && e.Status == "Active")
                    .OrderByDescending(e => e.EventDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching events: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EventListDto>> GetEventsWithParticipantCountAsync()
        {
            try
            {
                return await _dbSet
                    .Where(e => e.Status == "Active")
                    .Select(e => new EventListDto
                    {
                        EventId          = e.EventId,
                        EventName        = e.EventName,
                        EventCategory    = e.EventCategory,
                        EventDate        = e.EventDate,
                        Status           = e.Status,
                        Location         = e.Location,
                        MaxParticipants  = e.MaxParticipants,
                        ParticipantCount = e.ParticipantRegistrations.Count,
                        SessionCount     = e.Sessions.Count
                    })
                    .OrderBy(e => e.EventDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving events with participant count: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<EventListAdminDto>> GetAllEventsWithCountAsync()
        {
            try
            {
                // FIX: No status filter – admin must see ALL events (Active + Inactive)
                // so they can re-activate, delete, or audit any event.
                return await _dbSet
                    .Select(e => new EventListAdminDto
                    {
                        EventId           = e.EventId,
                        EventName         = e.EventName,
                        EventCategory     = e.EventCategory,
                        EventDate         = e.EventDate,
                        Status            = e.Status,
                        Location          = e.Location,
                        MaxParticipants   = e.MaxParticipants,
                        ParticipantCount  = e.ParticipantRegistrations.Count,
                        SessionCount      = e.Sessions.Count,
                        CreatedDate       = e.CreatedDate,
                        LastModifiedDate  = e.LastModifiedDate
                    })
                    .OrderByDescending(e => e.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all events for admin: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ToggleEventStatusAsync(Guid eventId)
        {
            try
            {
                if (eventId == Guid.Empty)
                    throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

                var ev = await _dbSet.FirstOrDefaultAsync(e => e.EventId == eventId);
                if (ev == null) return false;

                ev.Status           = ev.Status == "Active" ? "Inactive" : "Active";
                ev.LastModifiedDate = DateTime.UtcNow;

                _dbSet.Update(ev);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error toggling event status: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<string>> GetCategoriesAsync()
        {
            try
            {
                return await _dbSet
                    .Select(e => e.EventCategory)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving categories: {ex.Message}", ex);
            }
        }
    }
}