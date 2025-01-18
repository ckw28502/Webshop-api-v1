using System.Security.Cryptography;
using V1.Models;

namespace V1.Tests.RepositoryTests.UserRepositoryTests
{
    /// <summary>
    /// Contains unit tests for the EmailExists method of the <see cref="UserRepository"/> class.
    /// Tests ensure proper behavior when checking for email existence in the database.
    /// </summary>
    public class UserRepositoryEmailExistsTest : UserRepositoryTests
    {
        /// <summary>
        /// Verifies that <see cref="UserRepository.EmailExists"/> returns false
        /// when the specified email address does not exist in the database.
        /// </summary>
        [Fact]
        public async Task EmailExists_ReturnFalse_WhenEmailIsNotFound()
        {
            // Arrange
            string email = "not_found_email@email.com";

            // Act
            bool result = await _userRepository.EmailExists(email);

            // Assert
            Assert.False(result, "Expected EmailExists to return false for a non-existing email.");
        }

        /// <summary>
        /// Verifies that <see cref="UserRepository.EmailExists"/> returns true
        /// when the specified email address exists in the database.
        /// </summary>
        [Fact]
        public async Task EmailExists_ReturnTrue_WhenEmailIsFound()
        {
            // Arrange
            UserModel user = new()
            {
                Username = "found user",
                Email = "found_user@email.com",
                Password = "User1234",
                Salt = RandomNumberGenerator.GetBytes(16)
            };
            await _userRepository.CreateUser(user);

            // Act
            bool result = await _userRepository.EmailExists(user.Email);

            // Assert
            Assert.True(result, "Expected EmailExists to return true for an existing email.");
        }
    }
}
