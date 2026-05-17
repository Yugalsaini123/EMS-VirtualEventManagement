// EMS.DAL/Repository/SpeakerRepository.cs
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
    /// Contract for all speaker-related data operations.
    /// </summary>
    public interface ISpeakerRepository : IRepository<SpeakersDetails>
    {
        /// <summary>Get a speaker by their unique e-mail address.</summary>
        Task<SpeakersDetails?> GetByEmailAsync(string email);

        /// <summary>Return true if the e-mail is already registered to a speaker.</summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>Return all speakers whose IsActive flag is true.</summary>
        Task<IEnumerable<SpeakersDetails>> GetActiveSpeakersAsync();

        /// <summary>Case-insensitive name search over active speakers.</summary>
        Task<IEnumerable<SpeakersDetails>> SearchByNameAsync(string name);

        /// <summary>Return active speakers belonging to the specified organisation.</summary>
        Task<IEnumerable<SpeakersDetails>> GetByOrganizationAsync(string organization);

        /// <summary>
        /// Return active speakers with a count of their assigned sessions.
        /// FIX: was returning <c>dynamic</c> which breaks JSON serialisation in the
        /// API layer and prevents typed models in Angular.  Now returns a concrete
        /// <see cref="SpeakerSessionCountDto"/> list.
        /// </summary>
        Task<IEnumerable<SpeakerSessionCountDto>> GetSpeakersWithSessionCountAsync();

        /// <summary>Return all active speakers who have at least one session in the given event.</summary>
        Task<IEnumerable<SpeakersDetails>> GetSpeakersForEventAsync(Guid eventId);
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Implementation
    // ─────────────────────────────────────────────────────────────────────────────

    public class SpeakerRepository : Repository<SpeakersDetails>, ISpeakerRepository
    {
        public SpeakerRepository(EMSContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<SpeakersDetails?> GetByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be empty.", nameof(email));

                return await _dbSet.FirstOrDefaultAsync(s => s.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving speaker by email: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("Email cannot be empty.", nameof(email));

                return await _dbSet.AnyAsync(s => s.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking speaker email: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SpeakersDetails>> GetActiveSpeakersAsync()
        {
            try
            {
                return await _dbSet
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.SpeakerName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving active speakers: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SpeakersDetails>> SearchByNameAsync(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Search term cannot be empty.", nameof(name));

                return await _dbSet
                    .Where(s => s.SpeakerName.Contains(name) && s.IsActive)
                    .OrderBy(s => s.SpeakerName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching speakers: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SpeakersDetails>> GetByOrganizationAsync(string organization)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(organization))
                    throw new ArgumentException("Organisation cannot be empty.", nameof(organization));

                return await _dbSet
                    .Where(s => s.Organization == organization && s.IsActive)
                    .OrderBy(s => s.SpeakerName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving speakers by organisation: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SpeakerSessionCountDto>> GetSpeakersWithSessionCountAsync()
        {
            try
            {
                return await _dbSet
                    .Where(s => s.IsActive)
                    .Select(s => new SpeakerSessionCountDto
                    {
                        SpeakerId    = s.SpeakerId,
                        SpeakerName  = s.SpeakerName,
                        Email        = s.Email ?? string.Empty,
                        Designation  = s.Designation ?? string.Empty,
                        Organization = s.Organization ?? string.Empty,
                        SessionCount = s.Sessions.Count
                    })
                    .OrderBy(s => s.SpeakerName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving speakers with session count: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SpeakersDetails>> GetSpeakersForEventAsync(Guid eventId)
        {
            try
            {
                if (eventId == Guid.Empty)
                    throw new ArgumentException("Event ID cannot be empty.", nameof(eventId));

                return await _dbSet
                    .Where(s => s.IsActive && s.Sessions.Any(session => session.EventId == eventId))
                    .Distinct()
                    .OrderBy(s => s.SpeakerName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving speakers for event: {ex.Message}", ex);
            }
        }
    }
}