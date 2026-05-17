// EMS.Services\Implementations\UserService.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EMS.DAL.Models;
using EMS.DAL.Repository;
using EMS.Services.Interfaces;
using EMS.Services.Helpers;

namespace EMS.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, PasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<dynamic>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();

            return users.Select(u => (dynamic)new
            {
                u.EmailId,
                u.UserName,
                u.Role,
                u.IsActive,
                u.CreatedAt
            }).ToList();
        }

        public async Task<dynamic> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            return new
            {
                user.EmailId,
                user.UserName,
                user.Role,
                user.IsActive,
                user.CreatedAt
            };
        }

        public async Task<dynamic> UpdateUserAsync(string email, string userName, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("User name cannot be empty");

            user.UserName = userName;
            if (!string.IsNullOrWhiteSpace(password))
            {
                user.Password = _passwordHasher.HashPassword(password);
            }
            await _userRepository.UpdateAsync(user);

            return new
            {
                user.EmailId,
                user.UserName,
                user.Role
            };
        }

        public async Task<bool> DeleteUserAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Prevent deletion of last admin user
            if (user.Role == "Admin")
            {
                var adminUsers = await _userRepository.GetByRoleAsync("Admin");
                if (adminUsers.Count() == 1)
                    throw new InvalidOperationException("Cannot delete the last admin user");
            }

            await _userRepository.RemoveAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string email, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            // Verify old password
            if (!_passwordHasher.VerifyPassword(oldPassword, user.Password))
                throw new UnauthorizedAccessException("Current password is incorrect");

            // Validate new password
            if (newPassword == oldPassword)
                throw new ArgumentException("New password must be different from the current password");

            // Password validation: 8-20 chars, requires lowercase, uppercase, digit, special char
            var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$");
            if (!passwordRegex.IsMatch(newPassword))
                throw new ArgumentException("Password must be 8-20 characters and include lowercase, uppercase, digit, and special character");

            // Hash and update password
            user.Password = _passwordHasher.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<List<dynamic>> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepository.GetByRoleAsync(role);

            return users.Select(u => (dynamic)new
            {
                u.EmailId,
                u.UserName,
                u.Role,
                u.IsActive
            }).ToList();
        }

        public async Task<List<dynamic>> GetAllParticipantsAsync()
        {
            var participants = await _userRepository.GetAllParticipantsAsync();

            return participants.Select(u => (dynamic)new
            {
                u.EmailId,
                u.UserName,
                u.Role,
                u.IsActive,
                u.CreatedAt
            }).ToList();
        }
    }
}