using Moq;
using V1.DTOs;
using V1.Models;

namespace V1.Tests.ServiceTests.UserServiceTests
{
    /// <summary>
    /// Unit tests for the user registration functionality in the <see cref="UserService"/> class.
    /// </summary>
    public class RegisterUserTests : UserServiceTests
    {
        /// <summary>
        /// Test input data for creating a user, initialized with a valid username, email, and password.
        /// </summary>
        private readonly CreateUserDto _request;

        /// <summary>
        /// Initializes the test setup with test data for user registration.
        /// </summary>
        public RegisterUserTests()
        {
            // Initialize the CreateUserDto with sample input data for the tests
            _request = new CreateUserDto
            {
                Username = "user",
                Email = "user@email.com",
                Password = "User1234"
            };
        }

        /// <summary>
        /// Tests that an exception is thrown when attempting to create a user with a username that already exists.
        /// </summary>
        [Fact]
        public async Task CreateUser_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            // Arrange
            // Simulate that the username already exists in the repository
            _userRepositoryMock
                .Setup(repo => repo.UsernameExists(_request.Username))
                .ReturnsAsync(true);

            // Act & Assert
            // Verify that an exception is thrown when attempting to register the user
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.RegisterUser(_request)
            );

            // Assert that the exception message matches the expected error code for username existence
            Assert.Equal("USERNAME_EXISTS", exception.Message);

            // Verify that the UsernameExists method was called exactly once to check for the username
            _userRepositoryMock.Verify(repo => repo.UsernameExists(_request.Username), Times.Once);

            // Verify that EmailExists method was never called as the process should halt after the username check fails
            _userRepositoryMock.Verify(repo => repo.EmailExists(_request.Email), Times.Never);

            // Verify that the password hashing method was never called
            _passwordHasherMock.Verify(hasher => hasher.HashPassword(_request.Password), Times.Never);

            // Verify that the CreateUser method was never called
            _userRepositoryMock.Verify(repo => repo.CreateUser(It.IsAny<UserModel>()), Times.Never);
        }

        /// <summary>
        /// Tests that an exception is thrown when attempting to create a user with an email that already exists.
        /// </summary>
        [Fact]
        public async Task CreateUser_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            // Simulate that the username does not already exist in the repository
            _userRepositoryMock
                .Setup(repo => repo.UsernameExists(_request.Username))
                .ReturnsAsync(false);

            // Simulate that the email already exists in the repository
            _userRepositoryMock
                .Setup(repo => repo.EmailExists(_request.Email))
                .ReturnsAsync(true);

            // Act & Assert
            // Verify that an exception is thrown when attempting to register the user
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.RegisterUser(_request)
            );

            // Assert that the exception message matches the expected error code for email existence
            Assert.Equal("EMAIL_EXISTS", exception.Message);

            // Verify that the UsernameExists method was called exactly once to check for the username
            _userRepositoryMock.Verify(repo => repo.UsernameExists(_request.Username), Times.Once);

            // Verify that the EmailExists method was called exactly once to check for the email
            _userRepositoryMock.Verify(repo => repo.EmailExists(_request.Email), Times.Once);

            // Verify that the password hashing method was never called
            _passwordHasherMock.Verify(hasher => hasher.HashPassword(_request.Password), Times.Never);

            // Verify that the CreateUser method was never called
            _userRepositoryMock.Verify(repo => repo.CreateUser(It.IsAny<UserModel>()), Times.Never);
        }

        /// <summary>
        /// Tests that a user is successfully created when the username and email are valid and available.
        /// </summary>
        [Fact]
        public async Task CreateUser_ShouldCreateUser_WhenRequestIsValid()
        {
            // Arrange
            // Simulate that the username does not already exist in the repository
            _userRepositoryMock
                .Setup(repo => repo.UsernameExists(_request.Username))
                .ReturnsAsync(false);

            // Simulate that the email does not already exist in the repository
            _userRepositoryMock
                .Setup(repo => repo.EmailExists(_request.Email))
                .ReturnsAsync(false);

            // Simulate the password hashing process with predefined salt and hashed password
            byte[] salt = { 1, 2, 3, 4 }; // Sample salt for hashing
            string hashedPassword = "hashed password";

            _passwordHasherMock
                .Setup(hasher => hasher.HashPassword(_request.Password))
                .Returns((salt, hashedPassword));

            // Act
            // Call the RegisterUser method with valid input (non-existing username and email)
            await _userService.RegisterUser(_request);

            // Assert
            // Verify that UsernameExists was called exactly once to check the username availability
            _userRepositoryMock.Verify(repo => repo.UsernameExists(_request.Username), Times.Once);

            // Verify that EmailExists was called exactly once to check the email availability
            _userRepositoryMock.Verify(repo => repo.EmailExists(_request.Email), Times.Once);

            // Verify that HashPassword was called exactly once with the provided password
            _passwordHasherMock.Verify(hasher => hasher.HashPassword(_request.Password), Times.Once);

            // Verify that CreateUser was called exactly once to create the new user with the expected data
            _userRepositoryMock.Verify(repo => repo.CreateUser(It.IsAny<UserModel>()), Times.Once);
        }
    }
}
