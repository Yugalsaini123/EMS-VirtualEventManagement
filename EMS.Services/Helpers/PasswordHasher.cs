//EMS.Services/Helpers/PasswordHasher.cs
using System;
using System.Security.Cryptography;
using System.Text;

namespace EMS.Services.Helpers
{
    public class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int HashSize = 20; // 160 bit
        private const int Iterations = 10000;

        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt;
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt = new byte[SaltSize]);
            }

            // Create the Pbkdf2 hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(HashSize);

            // Combine salt and hash
            byte[] hashBytes = new byte[SaltSize + HashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

            // Convert to base64 string
            string passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                // Convert the hash string back to bytes
                byte[] hashBytes = Convert.FromBase64String(hash);

                // Get the salt
                byte[] salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                // Create the Pbkdf2 hash with the same salt
                var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                byte[] computedHash = pbkdf2.GetBytes(HashSize);

                // Compare the hashes
                for (int i = 0; i < HashSize; i++)
                {
                    if (hashBytes[i + SaltSize] != computedHash[i])
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
