using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using V1.Data;
using V1.Models;
using V1.Repositories;

namespace V1.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _userRepository;
        private readonly PostgresDbContext _dbContext;

        public UserRepositoryTests()
        {
            DbContextOptions<PostgresDbContext> options = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseInMemoryDatabase("testDatabase")  // Use In-Memory DB for testing
                .Options;
            
            _dbContext = new PostgresDbContext(options);

            _userRepository = new UserRepository(_dbContext);
        }

        [Fact]
        public async Task UsernameExists_ReturnFalse_WhenUsernameIsNotFound()
        {
            // Arrange
            UserModel user = new()
            {
                Username = "not found user",
                Password = "User1234",
                Salt = RandomNumberGenerator.GetBytes(1)
            };

            // Act
            bool result = await _userRepository.UsernameExists(user.Username);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UsernameExists_ReturnTrue_WhenUsernameIsFound()
        {
            // Arrange
            UserModel user = new()
            {
                Username = "found user",
                Password = "User1234",
                Salt = RandomNumberGenerator.GetBytes(1)
            };
            await _userRepository.CreateUser(user);

            // Act
            bool result = await _userRepository.UsernameExists(user.Username);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateUser_AddUserToDatabase()
        {
            // Arrange
            UserModel newUser = new()
            {
                Username = "new user",
                Password = "user",
                Salt = RandomNumberGenerator.GetBytes(1)
            };

            // Act
            await _userRepository.CreateUser(newUser);

            // Assert
            UserModel? userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == "new user");
            
            Assert.NotNull(userInDb);
            Assert.Equal(newUser.Username, userInDb?.Username);
            Assert.Equal(newUser.Password, userInDb?.Password);
            Assert.Equal(newUser.Salt, userInDb?.Salt);
        }
    }


}