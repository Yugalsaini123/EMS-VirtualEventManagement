// EMS.Services\Implementations\SessionService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.DAL.Models;
using EMS.DAL.Repository;
using EMS.Services.Interfaces;

namespace EMS.Services.Implementations
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ISpeakerRepository _speakerRepository;

        public SessionService(
            ISessionRepository sessionRepository,
            IEventRepository eventRepository,
            ISpeakerRepository speakerRepository)
        {
            _sessionRepository = sessionRepository;
            _eventRepository = eventRepository;
            _speakerRepository = speakerRepository;
        }


        private string CalculateSessionStatus(DateTime sessionStart, DateTime sessionEnd)
        {
            var now = DateTime.UtcNow;

            if (now < sessionStart)
                return "Upcoming";

            if (now >= sessionStart && now <= sessionEnd)
                return "Ongoing";

            return "Completed";
        }

        public async Task<List<dynamic>> GetSessionsByEventAsync(Guid eventId)
        {
            var sessions = await _sessionRepository.GetSessionsByEventAsync(eventId);

            return sessions.Select(s => (dynamic)new
            {
                s.SessionId,
                s.EventId,
                s.SessionTitle,
                s.Description,
                s.SessionStart,
                s.SessionEnd,
                s.Location,
                Status = CalculateSessionStatus(s.SessionStart, s.SessionEnd),
                s.SessionUrl,
                SpeakerId = s.SpeakerId,
                SpeakerName = s.Speaker?.SpeakerName ?? "TBA"
            }).ToList();
        }

        public async Task<List<dynamic>> SearchSessionsByEventAsync(Guid eventId, string searchTerm)
        {
            var sessions = await _sessionRepository.GetSessionsByEventAsync(eventId);

            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetSessionsByEventAsync(eventId);

            var filteredSessions = sessions.Where(s =>
                (s.SessionTitle ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (s.Description ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (s.Location ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            return filteredSessions.Select(s => (dynamic)new
            {
                s.SessionId,
                s.EventId,
                s.SessionTitle,
                s.Description,
                s.SessionStart,
                s.SessionEnd,
                s.Location,
                Status = CalculateSessionStatus(s.SessionStart, s.SessionEnd),
                s.SessionUrl,
                SpeakerId = s.SpeakerId,
                SpeakerName = s.Speaker?.SpeakerName ?? "TBA"
            }).ToList();
        }

        public async Task<dynamic> GetSessionByIdAsync(Guid sessionId)
        {
            var session = await _sessionRepository.GetSessionWithDetailsAsync(sessionId);

            if (session == null)
                throw new KeyNotFoundException("Session not found");

            return new
            {
                session.SessionId,
                session.EventId,
                session.SessionTitle,
                session.Description,
                session.SessionStart,
                session.SessionEnd,
                session.Location,
                Status = CalculateSessionStatus(session.SessionStart, session.SessionEnd),
                session.SessionUrl,
                SpeakerId = session.SpeakerId,
                SpeakerName = session.Speaker?.SpeakerName ?? "TBA"
            };
        }

        public async Task<dynamic> CreateSessionAsync(Guid eventId, string title, DateTime startTime,
            DateTime endTime, string description, string location, Guid? speakerId, string sessionUrl)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
                throw new ArgumentException("Session title must be at least 3 characters");

            if (string.IsNullOrWhiteSpace(description) || description.Length < 10)
                throw new ArgumentException("Description must be at least 10 characters");

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location is required");

            // Validate times
            if (startTime <= DateTime.UtcNow)
                throw new ArgumentException("Start time must be in the future");

            if (startTime >= endTime)
                throw new ArgumentException("Start time must be before end time");

            // Verify event exists
            var eventEntity = await _eventRepository.GetByIdAsync(eventId);
            if (eventEntity == null)
                throw new KeyNotFoundException("Event not found");

            // Sessions can be on any future date (not strictly tied to event date)
            // The event date is just the main event date, but sessions can span multiple days

            // Check for speaker conflicts if speakerId is provided
            if (speakerId.HasValue)
            {
                var speakerSessions = await _sessionRepository.GetSessionsBySpeakerAsync(speakerId.Value);
                
                // Check if speaker already has a session at this time
                foreach (var session in speakerSessions)
                {
                    if ((startTime >= session.SessionStart && startTime < session.SessionEnd) ||
                        (endTime > session.SessionStart && endTime <= session.SessionEnd) ||
                        (startTime <= session.SessionStart && endTime >= session.SessionEnd))
                    {
                        throw new InvalidOperationException("Speaker has a conflict during this time slot");
                    }
                }
            }

            var newSession = new SessionInfo
            {
                SessionId = Guid.NewGuid(),
                EventId = eventId,
                SessionTitle = title,
                Description = description,
                SessionStart = startTime,
                SessionEnd = endTime,
                Location = location,
                SpeakerId = speakerId,
                SessionUrl = sessionUrl,
                Status = CalculateSessionStatus(startTime, endTime),
                CreatedDate = DateTime.UtcNow
            };

            await _sessionRepository.AddAsync(newSession);

            return new
            {
                newSession.SessionId,
                newSession.EventId,
                newSession.SessionTitle,
                newSession.SessionUrl,
                Status = CalculateSessionStatus(newSession.SessionStart, newSession.SessionEnd)
            };
        }

        public async Task<dynamic> UpdateSessionAsync(Guid sessionId, string title, DateTime startTime,
            DateTime endTime, string description, string location, Guid? speakerId, string sessionUrl)
        {
            var session = await _sessionRepository.GetSessionWithDetailsAsync(sessionId);
            if (session == null)
                throw new KeyNotFoundException("Session not found");

            // Validate inputs
            if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
                throw new ArgumentException("Session title must be at least 3 characters");

            if (string.IsNullOrWhiteSpace(description) || description.Length < 10)
                throw new ArgumentException("Description must be at least 10 characters");

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Location is required");

            // Validate times
            if (startTime <= DateTime.UtcNow)
                throw new ArgumentException("Start time must be in the future");

            if (startTime >= endTime)
                throw new ArgumentException("Start time must be before end time");

            // Verify event exists
            var eventEntity = await _eventRepository.GetByIdAsync(session.EventId);
            if (eventEntity == null)
                throw new KeyNotFoundException("Event not found");

            // Sessions can be on any future date (not strictly tied to event date)

            // Check for speaker conflicts if speakerId changed or time changed
            if (speakerId.HasValue && (session.SpeakerId != speakerId || session.SessionStart != startTime || session.SessionEnd != endTime))
            {
                var speakerSessions = await _sessionRepository.GetSessionsBySpeakerAsync(speakerId.Value);

                // Check if speaker already has a session at this time (excluding current session)
                foreach (var s in speakerSessions.Where(s => s.SessionId != sessionId))
                {
                    if ((startTime >= s.SessionStart && startTime < s.SessionEnd) ||
                        (endTime > s.SessionStart && endTime <= s.SessionEnd) ||
                        (startTime <= s.SessionStart && endTime >= s.SessionEnd))
                    {
                        throw new InvalidOperationException("Speaker has a conflict during this time slot");
                    }
                }
            }

            session.SessionTitle = title;
            session.SessionStart = startTime;
            session.SessionEnd = endTime;
            session.Description = description;
            session.Location = location;
            session.SpeakerId = speakerId;
            session.SessionUrl = sessionUrl;
            session.Status = CalculateSessionStatus(startTime, endTime);
            session.LastModifiedDate = DateTime.UtcNow;

            await _sessionRepository.UpdateAsync(session);

            return new
            {
                session.SessionId,
                session.EventId,
                session.SessionTitle,
                session.SessionUrl,
                Status = CalculateSessionStatus(session.SessionStart, session.SessionEnd)
            };
        }

        public async Task<bool> DeleteSessionAsync(Guid sessionId)
        {
            var session = await _sessionRepository.GetSessionWithDetailsAsync(sessionId);
            if (session == null)
                throw new KeyNotFoundException("Session not found");

            // Cannot delete if session has already started
            if (session.SessionStart <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot delete sessions that have already started");

            await _sessionRepository.RemoveAsync(session);
            return true;
        }

        public async Task<bool> AssignSpeakerAsync(Guid sessionId, Guid speakerId)
        {
            return await _sessionRepository.AssignSpeakerAsync(sessionId, speakerId);
        }

        public async Task<bool> RemoveSpeakerAsync(Guid sessionId)
        {
            return await _sessionRepository.RemoveSpeakerAsync(sessionId);
        }

    }
}