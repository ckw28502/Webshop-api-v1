using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using V1.Models;
using V1.Repositories;

namespace V1.Tests.Repositories
{
    /// <summary>
    /// Unit tests for the UserRepository class, responsible for testing database operations
    /// related to user management.
    /// </summary>
    public class UserRepositoryTests : RepositoryTests
    {
        private readonly UserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the UserRepositoryTests class, setting up the UserRepository
        /// with an in-memory database to ensure isolated and repeatable tests.
        /// </summary>
        public UserRepositoryTests() : base()
        {
            _userRepository = new UserRepository(_dbContext);
        }

        /// <summary>
        /// Tests that UsernameExists returns false when the specified username does not exist in the database.
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
        /// Tests that UsernameExists returns true when the specified username exists in the database.
        /// </summary>
        [Fact]
        public async Task UsernameExists_ReturnTrue_WhenUsernameIsFound()
        {
            // Arrange
            UserModel user = new()
            {
                Username = "found user",
                Password = "User1234",
                Salt = RandomNumberGenerator.GetBytes(16)
            };
            await _userRepository.CreateUser(user);

            // Act
            bool result = await _userRepository.UsernameExists(user.Username);

            // Assert
            Assert.True(result, "Expected UsernameExists to return true for an existing username.");
        }

        /// <summary>
        /// Tests that CreateUser successfully inserts a new user record into the database.
        /// </summary>
        [Fact]
        public async Task CreateUser_AddUserToDatabase()
        {
            // Arrange
            UserModel newUser = new()
            {
                Username = "new user",
                Password = "user",
                Salt = RandomNumberGenerator.GetBytes(16)
            };

            // Act
            await _userRepository.CreateUser(newUser);

            // Assert
            UserModel? userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == "new user");

            Assert.NotNull(userInDb);
            Assert.Equal(newUser.Username, userInDb?.Username);
            Assert.Equal(newUser.Password, userInDb?.Password);

            byte[] saltInDb = userInDb?.Salt ?? [];
            Assert.True(newUser.Salt.SequenceEqual(saltInDb), "Expected the salt to match between the created user and the database entry.");
        }
    }
}
