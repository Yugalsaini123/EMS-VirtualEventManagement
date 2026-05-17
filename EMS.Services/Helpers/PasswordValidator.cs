using System;
using System.Collections.Generic;
using System.Linq;

namespace EMS.Services.Helpers
{
    public class PasswordValidator
    {
        private const int MinLength = 6;

        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
        }

        public static ValidationResult Validate(string password)
        {
            var result = new ValidationResult { IsValid = true, Errors = new List<string>() };

            if (string.IsNullOrWhiteSpace(password))
            {
                result.IsValid = false;
                result.Errors.Add("Password is required");
                return result;
            }

            // Check minimum length
            if (password.Length < MinLength)
            {
                result.IsValid = false;
                result.Errors.Add($"Password must be at least {MinLength} characters long");
            }

            // Check for lowercase letter
            if (!password.Any(char.IsLower))
            {
                result.IsValid = false;
                result.Errors.Add("Password must include at least one lowercase letter (a-z)");
            }

            // Check for uppercase letter
            if (!password.Any(char.IsUpper))
            {
                result.IsValid = false;
                result.Errors.Add("Password must include at least one uppercase letter (A-Z)");
            }

            // Check for digit
            if (!password.Any(char.IsDigit))
            {
                result.IsValid = false;
                result.Errors.Add("Password must include at least one digit (0-9)");
            }

            // Check for special character
            var specialChars = "!@#$%^&*()_+-=[]{}|;:',.<>?/\\`~";
            if (!password.Any(c => specialChars.Contains(c)))
            {
                result.IsValid = false;
                result.Errors.Add("Password must include at least one special character (!@#$%^&*()_+-=[]{}|;:',.<>?/\\`~)");
            }

            return result;
        }
    }
}
