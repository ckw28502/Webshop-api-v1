using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using V1.Models;

namespace V1.Tests.RepositoryTests.UserRepositoryTests
{
    /// <summary>
    /// Contains unit tests for the CreateUser method of the <see cref="UserRepository"/> class.
    /// Tests ensure proper behavior when adding new users to the database.
    /// </summary>
    public class UserRepositoryCreateUserTest : UserRepositoryTests
    {
        /// <summary>
        /// Verifies that <see cref="UserRepository.CreateUser"/> successfully inserts
        /// a new user record into the database.
        /// </summary>
        [Fact]
        public async Task CreateUser_AddUserToDatabase()
        {
            // Arrange
            UserModel newUser = new()
            {
                Username = "new user",
                Email = "new_user@email.com",
                Password = "user",
                Salt = RandomNumberGenerator.GetBytes(16)
            };

            // Act
            await _userRepository.CreateUser(newUser);

            // Assert
            UserModel? userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == "new user");

            Assert.NotNull(userInDb);
            Assert.Equal(newUser.Username, userInDb?.Username);
            Assert.Equal(newUser.Email, userInDb?.Email);
            Assert.Equal(newUser.Password, userInDb?.Password);

            byte[] saltInDb = userInDb?.Salt ?? [];
            Assert.True(newUser.Salt.SequenceEqual(saltInDb), "Expected the salt to match between the created user and the database entry.");
        }
    }
}
