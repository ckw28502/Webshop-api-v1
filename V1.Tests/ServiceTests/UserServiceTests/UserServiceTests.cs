using Moq;
using V1.Repositories;
using V1.Services;
using V1.Utils.EmailSender;
using V1.Utils.PasswordHasher;

namespace V1.Tests.ServiceTests.UserServiceTests
{
    /// <summary>
    /// Unit tests for the <see cref="UserService"/> class, focusing on the user-related logic.
    /// </summary>
    public abstract class UserServiceTests : ServiceTests
    {
        /// <summary>
        /// Mock of the <see cref="IUserRepository"/> used to simulate interactions with the user repository in tests.
        /// </summary>
        protected readonly Mock<IUserRepository> _userRepositoryMock;

        /// <summary>
        /// Mock of the <see cref="IPasswordHasher"/> used to simulate password hashing functionality in tests.
        /// </summary>
        protected readonly Mock<IPasswordHasher> _passwordHasherMock;

        /// <summary>
        /// Mock of the <see cref="IEmailSender"/> to simulate email sending functionality in the tests.
        /// </summary>
        protected readonly Mock<IEmailSender> _emailSenderMock;

        /// <summary>
        /// Instance of the <see cref="UserService"/> class being tested, initialized with the mocked dependencies.
        /// </summary>
        protected readonly UserService _userService;

        /// <summary>
        /// Initializes the test setup with mocked dependencies and prepares a test instance of <see cref="UserService"/>.
        /// This constructor is called once before each test method to ensure a fresh setup.
        /// </summary>
        public UserServiceTests()
        {
            // Mocking the IUserRepository to simulate interactions with the user repository.
            _userRepositoryMock = new Mock<IUserRepository>();

            // Mocking the IPasswordHasher to simulate password hashing functionality.
            _passwordHasherMock = new Mock<IPasswordHasher>();

            // Mock the IEmailSender to simulate sending emails during tests.
            _emailSenderMock = new Mock<IEmailSender>();

            // Initializing the UserService with mocked dependencies, making it ready for testing.
            _userService = new UserService(_userRepositoryMock.Object, _passwordHasherMock.Object, _emailSenderMock.Object);
        }

        /// <summary>
        /// Verifies that all mock interactions were performed as expected in the UserService test.
        /// This is called during the Dispose process in the base class.
        /// </summary>
        protected override void VerifyAllMocks()
        {
            // Verifying that all mock interactions have been completed as expected.
            _userRepositoryMock.VerifyAll();
            _passwordHasherMock.VerifyAll();
            _emailSenderMock.VerifyAll();
        }
    }
}
