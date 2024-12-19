using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using V1.Utils;

namespace V1.Tests.Utils
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_ShouldGenerateHashedPassword()
        {
            // Arrange
            string password = "Password123";

            // Act
            (byte[] salt, string hashedPassword) = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotNull(salt);
            Assert.Equal(128 / 8, salt.Length);
            Assert.NotEmpty(hashedPassword);
            Assert.NotEqual(password, hashedPassword);

            string expectedHashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password,
                salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            ));
            Assert.Equal(expectedHashedPassword, hashedPassword);
        }
    }
}