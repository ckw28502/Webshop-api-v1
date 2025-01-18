using V1.Repositories;

namespace V1.Tests.RepositoryTests.UserRepositoryTests
{
    /// <summary>
    /// Base class for unit tests targeting the <see cref="UserRepository"/> class.
    /// Provides foundational setup and configuration for testing database operations
    /// related to user management.
    /// </summary>
    public abstract class UserRepositoryTests : RepositoryTests
    {
        /// <summary>
        /// The instance of <see cref="UserRepository"/> used for testing.
        /// </summary>
        protected readonly UserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepositoryTests"/> class.
        /// Sets up the <see cref="UserRepository"/> with an in-memory database, ensuring
        /// isolated and repeatable tests for user-related database operations.
        /// </summary>
        public UserRepositoryTests() : base()
        {
            _userRepository = new UserRepository(_dbContext);
        }
    }
}
