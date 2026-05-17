// EMS.DAL/Repository/SessionRepository.cs
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
    /// Contract for all session-related data operations.
    /// </summary>
    public interface ISessionRepository : IRepository<SessionInfo>
    {
        /// <summary>Get a session by ID with its speaker loaded.</summary>
        Task<SessionInfo?> GetSessionWithDetailsAsync(Guid sessionId);

        /// <summary>Get all sessions belonging to an event, ordered by start time.</summary>
        Task<IEnumerable<SessionInfo>> GetSessionsByEventAsync(Guid eventId);

        /// <summary>Get all sessions assigned to a specific speaker.</summary>
        Task<IEnumerable<SessionInfo>> GetSessionsBySpeakerAsync(Guid speakerId);

        /// <summary>Get all Scheduled sessions whose start time is in the future.</summary>
        Task<IEnumerable<SessionInfo>> GetUpcomingSessionsAsync();

        /// <summary>
        /// Return true if the given speaker already has an overlapping session in the
        /// specified time window.  Pass <paramref name="excludeSessionId"/> when
        /// updating an existing session so it is not compared against itself.
        /// </summary>
        Task<bool> HasConflictingSessionAsync(
            Guid speakerId,
            DateTime sessionStart,
            DateTime sessionEnd,
            Guid? excludeSessionId = null);

        /// <summary>Assign a speaker to a session (checks for conflicts first).</summary>
        Task<bool> AssignSpeakerAsync(Guid sessionId, Guid speakerId);

        /// <summary>Remove the speaker assignment from a session.</summary>
        Task<bool> RemoveSpeakerAsync(Guid sessionId);

        /// <summary>Get sessions that have no speaker assigned yet.</summary>
        Task<IEnumerable<SessionInfo>> GetSessionsWithoutSpeakerAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Implementation
    // ─────────────────────────────────────────────────────────────────────────────

    public class SessionRepository : Repository<SessionInfo>, ISessionRepository
    {
        public SessionRepository(EMSContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<SessionInfo?> GetSessionWithDetailsAsync(Guid sessionId)
        {
            try
            {
                if (sessionId == Guid.Empty)
                    throw new ArgumentException("Session ID cannot be empty.", nameof(sessionId));

                return await _dbSet
                    .Include(s => s.Speaker)
                    .FirstOrDefaultAsync(s => s.SessionId == sessionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving session with details: {ex.Message}", ex);
            }
        }

        // NOTE: There is no GetByIdAsync(Guid) override here.
        //
        // FIX: The original code had a GetByIdAsync(Guid sessionId) method that
        // hid the base Repository<T>.GetByIdAsync(object id).  Because the base
        // interface declares GetByIdAsync(object), any code holding an
        // ISessionRepository reference and calling GetByIdAsync would call the
        // BASE method (which does NOT include the speaker), not the override.
        // The fix is to use GetSessionWithDetailsAsync(Guid) whenever the speaker
        // navigation property is needed, and let the base GetByIdAsync handle
        // plain ID lookups.  This eliminates the ambiguity entirely.

        /// <inheritdoc/>
        public async Task<IEnumerable<SessionInfo>> GetSessionsByEventAsync(Guid eventId)
        {
            try
            {
                if (eventId == Guid.Empty)
                    throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

                return await _dbSet
                    .Include(s => s.Speaker)
                    .Where(s => s.EventId == eventId)
                    .OrderBy(s => s.SessionStart)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sessions by event: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SessionInfo>> GetSessionsBySpeakerAsync(Guid speakerId)
        {
            try
            {
                if (speakerId == Guid.Empty)
                    throw new ArgumentException("Speaker ID cannot be empty.", nameof(speakerId));

                return await _dbSet
                    .Where(s => s.SpeakerId == speakerId)
                    .OrderBy(s => s.SessionStart)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sessions by speaker: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SessionInfo>> GetUpcomingSessionsAsync()
        {
            try
            {
                var now = DateTime.UtcNow;
                return await _dbSet
                    .Include(s => s.Speaker)
                    .Where(s => s.SessionStart >= now && s.Status == "Scheduled")
                    .OrderBy(s => s.SessionStart)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving upcoming sessions: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> HasConflictingSessionAsync(
            Guid speakerId,
            DateTime sessionStart,
            DateTime sessionEnd,
            Guid? excludeSessionId = null)
        {
            try
            {
                if (speakerId == Guid.Empty)
                    throw new ArgumentException("Speaker ID cannot be empty.", nameof(speakerId));

                var query = _dbSet.Where(s =>
                    s.SpeakerId == speakerId &&
                    s.SessionStart < sessionEnd &&
                    s.SessionEnd   > sessionStart);

                if (excludeSessionId.HasValue && excludeSessionId.Value != Guid.Empty)
                    query = query.Where(s => s.SessionId != excludeSessionId.Value);

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking session conflict: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> AssignSpeakerAsync(Guid sessionId, Guid speakerId)
        {
            try
            {
                if (sessionId == Guid.Empty || speakerId == Guid.Empty)
                    throw new ArgumentException("Session ID and Speaker ID cannot be empty.");

                var session = await _dbSet.FirstOrDefaultAsync(s => s.SessionId == sessionId);
                if (session == null) return false;

                if (await HasConflictingSessionAsync(speakerId, session.SessionStart, session.SessionEnd, sessionId))
                    throw new InvalidOperationException("Speaker already has a session scheduled in this time slot.");

                session.SpeakerId         = speakerId;
                session.LastModifiedDate  = DateTime.UtcNow;
                _dbSet.Update(session);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error assigning speaker: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveSpeakerAsync(Guid sessionId)
        {
            try
            {
                if (sessionId == Guid.Empty)
                    throw new ArgumentException("Session ID cannot be empty.", nameof(sessionId));

                var session = await _dbSet.FirstOrDefaultAsync(s => s.SessionId == sessionId);
                if (session == null) return false;

                session.SpeakerId        = null;
                session.LastModifiedDate = DateTime.UtcNow;
                _dbSet.Update(session);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error removing speaker: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SessionInfo>> GetSessionsWithoutSpeakerAsync()
        {
            try
            {
                return await _dbSet
                    .Where(s => s.SpeakerId == null)
                    .OrderBy(s => s.SessionStart)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving sessions without speaker: {ex.Message}", ex);
            }
        }
    }
}