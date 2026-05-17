//EMS.Services/Implementations/AuthService.cs
using System;
using System.Threading.Tasks;
using EMS.DAL.Models;
using EMS.DAL.Repository;
using EMS.Services.Interfaces;
using EMS.Services.Helpers;

namespace EMS.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly PasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepository, JwtTokenHelper jwtTokenHelper, PasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _jwtTokenHelper = jwtTokenHelper;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email and password are required");
        
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");
        
            // Verify hashed password
            if (!_passwordHasher.VerifyPassword(password, user.Password))
                throw new UnauthorizedAccessException("Invalid email or password");
        
            var token = _jwtTokenHelper.GenerateToken(user.EmailId, user.UserName, user.Role);
            return token;
        }
        

        public async Task<bool> RegisterAsync(string email, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Email, username, and password are required");

            // Validate password requirements
            var passwordValidation = PasswordValidator.Validate(password);
            if (!passwordValidation.IsValid)
            {
                var errorMessages = string.Join("; ", passwordValidation.Errors);
                throw new ArgumentException(errorMessages);
            }

            // Check if email already exists
            var emailExists = await _userRepository.EmailExistsAsync(email);
            if (emailExists)
                throw new InvalidOperationException($"Email '{email}' is already registered");

            // Check if username already exists
            var usernameExists = await _userRepository.UsernameExistsAsync(userName);
            if (usernameExists)
                throw new InvalidOperationException($"Username '{userName}' is already taken");

            var newUser = new UserInfo
            {
                EmailId = email,
                UserName = userName,
                Password = _passwordHasher.HashPassword(password),
                Role = "Participant",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(newUser);
            return true;
        }

        public async Task<string> RefreshTokenAsync(string oldToken)
        {
            var email = _jwtTokenHelper.GetEmailFromToken(oldToken);
            if (string.IsNullOrWhiteSpace(email))
                throw new UnauthorizedAccessException("Invalid token");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            var newToken = _jwtTokenHelper.GenerateToken(user.EmailId, user.UserName, user.Role);
            return newToken;
        }

        public async Task<bool> LogoutAsync(string email)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.AuthenticateAsync(email, password);
            return user != null;
        }
    }
}