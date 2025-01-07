using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using V1.Models;

namespace V1.Tests.RepositoryTests.UserRepositoryTests
{
    /// <summary>
    /// Contains unit tests for the <see cref="UserRepository.UsernameExists"/> method.
    /// Tests verify behavior when checking for existing or non-existing usernames in the database.
    /// </summary>
    public class UserRepositoryUsernameExistsTest : UserRepositoryTests
    {
        /// <summary>
        /// Verifies that <see cref="UserRepository.UsernameExists"/> returns false
        /// when the specified username does not exist in the database.
        /// </summary>
        [Fact]
        public async Task UsernameExists_ReturnFalse_WhenUsernameIsNotFound()
        {
            // Arrange
            string username = "not found user";

            // Act
            bool result = await _userRepository.UsernameExists(username);

            // Assert
            Assert.False(result, "Expected UsernameExists to return false for a non-existing username.");
        }

        /// <summary>
        /// Verifies that <see cref="UserRepository.UsernameExists"/> returns true
        /// when the specified username exists in the database.
        /// </summary>
        [Fact]
        public async Task UsernameExists_ReturnTrue_WhenUsernameIsFound()
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
            bool result = await _userRepository.UsernameExists(user.Username);

            // Assert
            Assert.True(result, "Expected UsernameExists to return true for an existing username.");
        }
    }
}
