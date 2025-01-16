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
        /// <remarks>
        /// This test simulates a scenario where the username provided by the user already exists in the repository.
        /// The test verifies that an exception is thrown with the correct error code ("USERNAME_EXISTS") and ensures
        /// that no further repository methods are called after the failure.
        /// </remarks>
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

            // Verify that the email was never sent
            _emailSenderMock.Verify(mailSender => mailSender.SendEmail(_request.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Tests that an exception is thrown when attempting to create a user with an email that already exists.
        /// </summary>
        /// <remarks>
        /// This test simulates a scenario where the email provided by the user already exists in the repository.
        /// The test verifies that an exception is thrown with the correct error code ("EMAIL_EXISTS") and ensures
        /// that no further repository methods are called after the failure.
        /// </remarks>
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

            // Verify that the email was never sent
            _emailSenderMock.Verify(mailSender => mailSender.SendEmail(_request.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Mocks the password hashing behavior to simulate the password hashing process.
        /// </summary>
        private void MockHashPassword()
        {
            byte[] salt = [1, 2, 3, 4];
            string hashedPassword = "hashed password";

            _passwordHasherMock
                .Setup(hasher => hasher.HashPassword(_request.Password))
                .Returns((salt, hashedPassword));
        }

        /// <summary>
        /// Tests that a user registration process correctly handles exceptions, rolls back transactions, and verifies method calls.
        /// </summary>
        /// <param name="createUserThrows">Flag to determine if the CreateUser method should throw an exception.</param>
        /// <param name="sendEmailThrows">Flag to determine if the SendEmail method should throw an exception.</param>
        /// <remarks>
        /// This test verifies that the user creation process rolls back the transaction if an error occurs during user creation or email sending.
        /// The test covers two scenarios:
        /// 1. An error occurs during user creation.
        /// 2. An error occurs during email sending after the user is created.
        /// </remarks>
        [Theory]
        [InlineData(true, false)]  // Case where CreateUser throws exception, SendEmail does not
        [InlineData(false, true)]  // Case where SendEmail throws exception, CreateUser does not
        public async Task CreateUser_ShouldRollbackTransaction_WhenAnErrorOccurs(bool createUserThrows, bool sendEmailThrows)
        {
            // Arrange:
            // Simulate that the username and email do not already exist in the repository.
            _userRepositoryMock
                .Setup(repo => repo.UsernameExists(_request.Username))
                .ReturnsAsync(false);

            _userRepositoryMock
                .Setup(repo => repo.EmailExists(_request.Email))
                .ReturnsAsync(false);

            MockHashPassword();

            // Mock StartTransaction to return a mock transaction for testing.
            _userRepositoryMock
                .Setup(repo => repo.StartTransaction())
                .ReturnsAsync(_transactionMock.Object);

            // Set up CreateUser to throw an exception if required.
            if (createUserThrows)
            {
                _userRepositoryMock
                    .Setup(repo => repo.CreateUser(It.IsAny<UserModel>()))
                    .ThrowsAsync(new Exception("Database error"));
            }
            else
            {
                _userRepositoryMock
                    .Setup(repo => repo.CreateUser(It.IsAny<UserModel>()))
                    .Returns(Task.CompletedTask); // Simulate successful user creation

                // Set up SendEmail to throw an exception if required.
                if (sendEmailThrows)
                {
                    _emailSenderMock
                        .Setup(mailSender => mailSender.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ThrowsAsync(new Exception("Email sending error"));
                }
                else
                {
                    _emailSenderMock
                        .Setup(mailSender => mailSender.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .Returns(Task.CompletedTask); // Simulate successful email sending
                }
            }

            // Act:
            // Try to call RegisterUser, expecting an exception to be thrown depending on the scenario.
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.RegisterUser(_request));

            // Assert:
            // Verify that UsernameExists was called exactly once.
            _userRepositoryMock.Verify(repo => repo.UsernameExists(_request.Username), Times.Once);

            // Verify that EmailExists was called exactly once.
            _userRepositoryMock.Verify(repo => repo.EmailExists(_request.Email), Times.Once);

            // Verify that HashPassword was called exactly once.
            _passwordHasherMock.Verify(hasher => hasher.HashPassword(_request.Password), Times.Once);

            // Verify that CreateUser was called once.
            _userRepositoryMock.Verify(repo => repo.CreateUser(It.IsAny<UserModel>()), Times.Once);

            // Verify that SendEmail was called once, if no exception occurs.
            _emailSenderMock.Verify(mailSender => mailSender.SendEmail(_request.Email, It.IsAny<string>(), It.IsAny<string>()), sendEmailThrows ? Times.Once : Times.Never);

            // Verify that CommitAsync was not called when an exception occurs.
            _transactionMock.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

            // Verify that RollbackAsync was called once to roll back the transaction due to the exception.
            _transactionMock.Verify(transaction => transaction.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that a user is successfully created when the username and email are valid and available.
        /// </summary>
        /// <remarks>
        /// This test simulates a successful user registration where the username and email are available.
        /// The test verifies that the user is created, password hashing is done, and an email is sent.
        /// It also ensures that the repository methods for checking availability and creating the user are called as expected.
        /// </remarks>
        [Fact]
        public async Task CreateUser_ShouldCreateUser_WhenRequestIsValid()
        {
            // Simulate that the username does not already exist in the repository
            _userRepositoryMock
                .Setup(repo => repo.UsernameExists(_request.Username))
                .ReturnsAsync(false);

            // Simulate that the email does not already exist in the repository
            _userRepositoryMock
                .Setup(repo => repo.EmailExists(_request.Email))
                .ReturnsAsync(false);

            // Mock StartTransaction to return a mock transaction
            _userRepositoryMock
                .Setup(repo => repo.StartTransaction())
                .ReturnsAsync(_transactionMock.Object);

            MockHashPassword();

            // Act
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

            // Verify that the email was sent once
            _emailSenderMock.Verify(mailSender => mailSender.SendEmail(_request.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            // Verify that the transaction's CommitAsync method was called once to commit the changes to the database.
            _transactionMock.Verify(transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

            // Verify that the transaction's RollbackAsync method was never called, indicating that no error occurred and the transaction was committed.
            _transactionMock.Verify(transaction => transaction.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
