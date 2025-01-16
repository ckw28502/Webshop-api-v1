using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace V1.Tests.ServiceTests
{
    /// <summary>
    /// Base class for all service tests, managing the setup, cleanup, and mock verification logic for service-based tests.
    /// Implements <see cref="IDisposable"/> to ensure resources are properly cleaned up after each test execution.
    /// </summary>
    public abstract class ServiceTests : IDisposable
    {
        /// <summary>
        /// Mock instance of <see cref="IDbContextTransaction"/> to simulate database transactions in service tests.
        /// </summary>
        protected Mock<IDbContextTransaction> _transactionMock;

        /// <summary>
        /// Initializes the test setup by creating the mock instances required for tests.
        /// This constructor is called before each test method to ensure that a fresh setup is provided.
        /// </summary>
        public ServiceTests()
        {
            // Initialize the mock transaction to simulate database transaction behavior during tests.
            _transactionMock = new Mock<IDbContextTransaction>();
        }

        /// <summary>
        /// Disposes of the resources used by the test class.
        /// Ensures proper cleanup of resources after each test execution.
        /// It also verifies that all mock interactions were completed as expected.
        /// </summary>
        public void Dispose()
        {
            // Verify that all mock interactions were performed as expected during the tests.
            VerifyAllMocks();

            // Suppress finalization to prevent unnecessary finalization of this object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Abstract method that should be implemented in derived classes to verify that all mock interactions
        /// have been completed as expected during the tests. This method is called during the Dispose process.
        /// </summary>
        protected abstract void VerifyAllMocks();
    }
}
