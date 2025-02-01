using Moq;
using V1.DTOs;
using V1.Models;

namespace V1.Tests.ServiceTests.UserServiceTests
{
    /// <summary>
    /// Unit tests for the user registration functionality in the <see cref="UserService"/> class.
    /// These tests validate different registration scenarios, including success, validation failures,
    /// and transactional rollbacks in case of errors.
    /// </summary>
    public class RegisterUserTests : UserServiceTests
    {
        /// <summary>
        /// Test data for user registration, initialized with valid input values.
        /// </summary>
        private readonly CreateUserDto _request;

        /// <summary>
        /// Initializes the test setup with predefined user registration data.
        /// </summary>
        public RegisterUserTests()
        {
            _request = new CreateUserDto
            {
                Username = "user",
                Email = "user@email.com",
                Password = "User1234"
            };
        }

        /// <summary>
        /// Ensures that attempting to register a user with an existing username results in an exception.
        /// </summary>
        [Fact]
        public async Task CreateUser_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            // Arrange: Mock repository to simulate existing username
            _userRepositoryMock.Setup(repo => repo.UsernameExists(_request.Username)).ReturnsAsync(true);

            // Act & Assert: Expect an exception with the "USERNAME_EXISTS" error code
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUser(_request));
            Assert.Equal("USERNAME_EXISTS", exception.Message);

            // Verify repository method calls
            _userRepositoryMock.Verify(repo => repo.UsernameExists(_request.Username), Times.Once);
            _userRepositoryMock.Verify(repo => repo.EmailExists(_request.Email), Times.Never);
            _passwordHasherMock.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.CreateUser(It.IsAny<UserModel>()), Times.Never);
            _emailSenderMock.Verify(mailSender => mailSender.SendUserVerificationEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Ensures that attempting to register a user with an existing email results in an exception.
        /// </summary>
        [Fact]
        public async Task CreateUser_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange: Mock repository to simulate existing email
            _userRepositoryMock.Setup(repo => repo.UsernameExists(_request.Username)).ReturnsAsync(false);
            _userRepositoryMock.Setup(repo => repo.EmailExists(_request.Email)).ReturnsAsync(true);

            // Act & Assert: Expect an exception with the "EMAIL_EXISTS" error code
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RegisterUser(_request));
            Assert.Equal("EMAIL_EXISTS", exception.Message);

            // Verify repository method calls
            _userRepositoryMock.Verify(repo => repo.UsernameExists(_request.Username), Times.Once);
            _userRepositoryMock.Verify(repo => repo.EmailExists(_request.Email), Times.Once);
            _passwordHasherMock.Verify(hasher => hasher.HashPassword(It.IsAny<string>()), Times.Never);
            _userRepositoryMock.Verify(repo => repo.CreateUser(It.IsAny<UserModel>()), Times.Never);
            _emailSenderMock.Verify(mailSender => mailSender.SendUserVerificationEmail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Simulates password hashing behavior to test the registration process.
        /// </summary>
        private void MockHashPassword()
        {
            byte[] salt = [1, 2, 3, 4];
            string hashedPassword = "hashed_password";
            _passwordHasherMock.Setup(hasher => hasher.HashPassword(_request.Password)).Returns((salt, hashedPassword));
        }

        /// <summary>
        /// Ensures that a transaction is rolled back when an error occurs during user creation or email sending.
        /// </summary>
        [Theory]
        [InlineData(true, false)] // Simulate failure in CreateUser
        [InlineData(false, true)] // Simulate failure in SendEmail
        public async Task CreateUser_ShouldRollbackTransaction_WhenAnErrorOccurs(bool createUserThrows, bool sendEmailThrows)
        {
            // Arrange: Setup repository mocks
            _userRepositoryMock.Setup(repo => repo.UsernameExists(_request.Username)).ReturnsAsync(false);
            _userRepositoryMock.Setup(repo => repo.EmailExists(_request.Email)).ReturnsAsync(false);
            MockHashPassword();
            _userRepositoryMock.Setup(repo => repo.StartTransaction()).ReturnsAsync(_transactionMock.Object);

            // Simulate errors
            if (createUserThrows)
                _userRepositoryMock.Setup(repo => repo.CreateUser(It.IsAny<UserModel>())).ThrowsAsync(new Exception("Database error"));
            else if (sendEmailThrows)
                _emailSenderMock.Setup(mailSender => mailSender.SendUserVerificationEmail(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception("Email error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.RegisterUser(_request));

            // Verify rollback and method calls
            _transactionMock.Verify(transaction => transaction.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            _transactionMock.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Ensures that a user is successfully created when all validation checks pass.
        /// </summary>
        [Fact]
        public async Task CreateUser_ShouldCreateUser_WhenRequestIsValid()
        {
            // Arrange: Mock repository behavior for a valid registration
            _userRepositoryMock.Setup(repo => repo.UsernameExists(_request.Username)).ReturnsAsync(false);
            _userRepositoryMock.Setup(repo => repo.EmailExists(_request.Email)).ReturnsAsync(false);
            _userRepositoryMock.Setup(repo => repo.StartTransaction()).ReturnsAsync(_transactionMock.Object);
            MockHashPassword();

            // Act
            await _userService.RegisterUser(_request);

            // Assert: Verify expected repository method calls
            _userRepositoryMock.Verify(repo => repo.UsernameExists(_request.Username), Times.Once);
            _userRepositoryMock.Verify(repo => repo.EmailExists(_request.Email), Times.Once);
            _passwordHasherMock.Verify(hasher => hasher.HashPassword(_request.Password), Times.Once);
            _userRepositoryMock.Verify(repo => repo.CreateUser(It.IsAny<UserModel>()), Times.Once);
            _emailSenderMock.Verify(mailSender => mailSender.SendUserVerificationEmail(_request.Email, It.IsAny<string>()), Times.Once);
            _transactionMock.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
