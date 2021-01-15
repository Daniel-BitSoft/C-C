using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;

namespace CC.Providers
{
    public class CryptoProvider
    {
        public const int SaltByteSize = 16;
        public const int Pbkdf2Iterations = 20000;

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltByteSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Pbkdf2Iterations,
                numBytesRequested: 256 / 8));

            return $"{hashed}:{Convert.ToBase64String(salt)}";
        }

        public static bool ValidatePassword(string inputPassword, string storedPassword)
        {
            var passwordParts = storedPassword.Split(new char[] { ':' });
            var saltBytes = Convert.FromBase64String(passwordParts[1]);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: inputPassword,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: Pbkdf2Iterations,
                numBytesRequested: 256 / 8));

            return hashed == passwordParts[0];
        }
    }
}
