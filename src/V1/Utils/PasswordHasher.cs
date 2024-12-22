using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace V1.Utils
{
    /// <summary>
    /// Provides functionality for hashing passwords securely using a salt and PBKDF2.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        /// <summary>
        /// Hashes the given password using a randomly generated salt and PBKDF2.
        /// </summary>
        /// <param name="password">The plaintext password to hash.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        ///   <item><description>The generated salt as a byte array.</description></item>
        ///   <item><description>The hashed password as a Base64-encoded string.</description></item>
        /// </list>
        /// </returns>
        public (byte[] salt, string hashedPassword) HashPassword(string password)
        {
            // Generate a random 128-bit salt (16 bytes)
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

            // Hash the password using the generated salt
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,         // The password to hash
                salt: salt,                 // The salt to use in the hashing process
                prf: KeyDerivationPrf.HMACSHA256, // The pseudo-random function for hashing
                iterationCount: 100000,     // The number of iterations to strengthen the hash
                numBytesRequested: 256 / 8  // The size of the resulting hash (32 bytes for 256-bit hash)
            ));

            // Return the salt and hashed password
            return (salt, hashedPassword);
        }
    }
}
