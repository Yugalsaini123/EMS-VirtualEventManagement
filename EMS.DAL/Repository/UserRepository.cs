// EMS.DAL/Repository/UserRepository.cs
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
    /// Contract for all user account operations.
    /// </summary>
    public interface IUserRepository : IRepository<UserInfo>
    {
        /// <summary>Get a user by their e-mail address (primary key).</summary>
        Task<UserInfo?> GetByEmailAsync(string emailId);

        /// <summary>Return true if an account with this e-mail already exists.</summary>
        Task<bool> EmailExistsAsync(string emailId);

        /// <summary>Return true if an account with this username already exists.</summary>
        Task<bool> UsernameExistsAsync(string userName);

        /// <summary>Return all active users belonging to the specified role.</summary>
        Task<IEnumerable<UserInfo>> GetByRoleAsync(string role);

        /// <summary>
        /// Validate credentials. Returns the <see cref="UserInfo"/> on success or
        /// <c>null</c> if the credentials are wrong / the account is inactive.
        /// </summary>
        Task<UserInfo?> AuthenticateAsync(string emailId, string password);

        /// <summary>Update the password for an existing user account.</summary>
        Task<bool> UpdatePasswordAsync(string emailId, string newPassword);

        /// <summary>Return the count of accounts where IsActive == true.</summary>
        Task<int> GetActiveUsersCountAsync();

        /// <summary>Return all active Participant-role accounts, newest first.</summary>
        Task<IEnumerable<UserInfo>> GetAllParticipantsAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────────
    // Implementation
    // ─────────────────────────────────────────────────────────────────────────────

    public class UserRepository : Repository<UserInfo>, IUserRepository
    {
        public UserRepository(EMSContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<UserInfo?> GetByEmailAsync(string emailId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId))
                    throw new ArgumentException("Email ID cannot be empty.", nameof(emailId));

                return await _dbSet.FirstOrDefaultAsync(u => u.EmailId == emailId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user by email: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> EmailExistsAsync(string emailId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId))
                    throw new ArgumentException("Email ID cannot be empty.", nameof(emailId));

                return await _dbSet.AnyAsync(u => u.EmailId == emailId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking email existence: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UsernameExistsAsync(string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName))
                    throw new ArgumentException("Username cannot be empty.", nameof(userName));

                return await _dbSet.AnyAsync(u => u.UserName == userName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking username existence: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserInfo>> GetByRoleAsync(string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role))
                    throw new ArgumentException("Role cannot be empty.", nameof(role));

                return await _dbSet
                    .Where(u => u.Role == role && u.IsActive)
                    .OrderBy(u => u.UserName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving users by role: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<UserInfo?> AuthenticateAsync(string emailId, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId) || string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Email and password are required.");

                // NOTE: In a production system passwords would be hashed (BCrypt / Argon2).
                // Plain-text comparison is acceptable only for this prototype.
                var user = await _dbSet.FirstOrDefaultAsync(u => u.EmailId == emailId);

                if (user == null || user.Password != password || !user.IsActive)
                    return null;

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error authenticating user: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdatePasswordAsync(string emailId, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailId) || string.IsNullOrWhiteSpace(newPassword))
                    throw new ArgumentException("Email and password are required.");

                var user = await _dbSet.FirstOrDefaultAsync(u => u.EmailId == emailId);
                if (user == null) return false;

                user.Password = newPassword;
                _dbSet.Update(user);
                await SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating password: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<int> GetActiveUsersCountAsync()
        {
            try
            {
                return await _dbSet.CountAsync(u => u.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting active users: {ex.Message}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UserInfo>> GetAllParticipantsAsync()
        {
            try
            {
                return await _dbSet
                    .Where(u => u.Role == "Participant" && u.IsActive)
                    .OrderByDescending(u => u.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving participants: {ex.Message}", ex);
            }
        }
    }
}