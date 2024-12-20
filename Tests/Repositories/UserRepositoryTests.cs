using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using V1.Data;
using V1.Models;
using V1.Repositories;

namespace V1.Tests.Repositories
{
    /// <summary>
    /// Unit tests for the UserRepository class.
    /// </summary>
    public class UserRepositoryTests
    {
        private readonly UserRepository _userRepository;
        private readonly PostgresDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of UserRepositoryTests, setting up an in-memory database for testing.
        /// </summary>
        public UserRepositoryTests()
        {
            DbContextOptions<PostgresDbContext> options = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseInMemoryDatabase("testDatabase") // Use In-Memory DB for testing
                .Options;

            _dbContext = new PostgresDbContext(options);
            _userRepository = new UserRepository(_dbContext);
        }

        /// <summary>
        /// Verifies that UsernameExists returns false when the username is not found in the database.
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
        /// Verifies that UsernameExists returns true when the username is found in the database.
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
        /// Verifies that CreateUser successfully adds a user to the database.
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
