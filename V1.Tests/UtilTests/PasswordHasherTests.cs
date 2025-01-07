using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using V1.Utils.PasswordHasher;

namespace V1.Tests.UtilTests
{
    /// <summary>
    /// Unit tests for the PasswordHasher utility class.
    /// </summary>
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        /// <summary>
        /// Initializes the test setup with an instance of PasswordHasher.
        /// </summary>
        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        /// <summary>
        /// Tests that the HashPassword method generates a valid hashed password and salt.
        /// </summary>
        [Fact]
        public void HashPassword_ShouldGenerateHashedPassword()
        {
            // Arrange
            string password = "Password123";

            // Act
            (byte[] salt, string hashedPassword) = _passwordHasher.HashPassword(password);

            // Assert
            // Ensure the salt is not null and has the expected length
            Assert.NotNull(salt);
            Assert.Equal(128 / 8, salt.Length); // Salt length should be 16 bytes (128 bits)

            // Ensure the hashed password is not empty and differs from the original password
            Assert.NotEmpty(hashedPassword);
            Assert.NotEqual(password, hashedPassword);

            // Verify the hashed password matches the expected hash using the same salt and parameters
            string expectedHashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8 // 32 bytes (256 bits) for the hashed password
            ));
            Assert.Equal(expectedHashedPassword, hashedPassword);
        }
    }
}
