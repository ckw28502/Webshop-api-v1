namespace V1.Tests.ServiceTests
{
    /// <summary>
    /// Base class for all service tests, handling the setup, cleanup, and mock verification logic for all service-based tests.
    /// This class implements IDisposable to ensure that resources are cleaned up after each test.
    /// </summary>
    public abstract class ServiceTests : IDisposable
    {
        /// <summary>
        /// Disposes of the resources used by the test class.
        /// This method ensures that any resources that require cleanup are disposed of correctly.
        /// It also verifies that all mock interactions were completed as expected.
        /// </summary>
        public void Dispose()
        {
            // Verify that all expected mock interactions were performed
            VerifyAllMocks();

            // Suppress finalization to prevent calling the finalizer
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Verifies that all mock objects have been interacted with as expected.
        /// This method is called during the dispose process to ensure that mocks were used correctly.
        /// </summary>
        protected abstract void VerifyAllMocks();
    }
}
