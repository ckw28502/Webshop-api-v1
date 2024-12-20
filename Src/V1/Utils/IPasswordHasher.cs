namespace V1.Utils
{
    /// <summary>
    /// Defines the contract for a password hashing service.
    /// This interface provides the method for hashing a plain password
    /// and returning the corresponding salt and hashed password.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes the provided plain password and returns the salt and hashed password.
        /// The method uses a cryptographic algorithm to generate a secure hash
        /// and a salt for the password.
        /// </summary>
        /// <param name="plainPassword">The plain password to be hashed.</param>
        /// <returns>
        /// A tuple containing:
        /// - A <see cref="byte[]"/> representing the salt used in the hashing process.
        /// - A <see cref="string"/> representing the hashed password.
        /// </returns>
        /// <remarks>
        /// The salt should be stored securely in the database to ensure each password is hashed with a unique value,
        /// even if users share the same password. This method employs the PBKDF2 algorithm with HMACSHA256.
        /// </remarks>
        (byte[] salt, string hashedPassword) HashPassword(string plainPassword);
    }
}
