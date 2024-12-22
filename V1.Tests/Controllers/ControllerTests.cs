using Moq;
using V1.Tests.TestUtilities;

namespace V1.Tests.Controllers
{
    /// <summary>
    /// Base test class for controller tests, providing the necessary setup for integration testing.
    /// This abstract class is designed to be inherited by controller test classes and configures
    /// the testing environment, including mocking services and setting up an HTTP client for tests.
    /// It simplifies the process of writing integration tests for controllers by providing an 
    /// in-memory test server, mocked service, and HttpClient for sending requests.
    /// </summary>
    public abstract class ControllerTests<TService> : IDisposable where TService : class
    {
        /// <summary>
        /// HttpClient used to send HTTP requests to the in-memory test server for performing integration tests.
        /// The HttpClient interacts with the in-memory server created by the custom web application factory 
        /// for testing API calls.
        /// </summary>
        protected readonly HttpClient _client;

        /// <summary>
        /// Mock instance of the service being tested.
        /// This mock is used to simulate the behavior of the service during tests by setting up expectations 
        /// and controlling the responses of service methods.
        /// </summary>
        protected readonly Mock<TService> _serviceMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerTests{TService}"/> class.
        /// This constructor sets up the testing environment by:
        /// - Mocking the service being tested.
        /// - Initializing the custom web application factory with the mocked service.
        /// - Creating an HttpClient that can be used to send HTTP requests to the in-memory test server.
        /// 
        /// This setup allows the derived test classes to focus on testing the controller actions 
        /// without worrying about the configuration of the server or service.
        /// </summary>
        public ControllerTests()
        {
            // Set environment variable for testing
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            // Create a mock instance of the service passed as a generic parameter.
            _serviceMock = new Mock<TService>();

            // Initialize the custom web application factory with the mocked service.
            // The factory is responsible for setting up the in-memory test server with the mock service.
            CustomWebApplicationFactory<TService> _factory = new(_serviceMock.Object);

            // Create an HttpClient that will be used to send requests to the in-memory test server.
            _client = _factory.CreateClient();
        }

        /// <summary>
        /// Disposes of the resources used by the test class.
        /// This method is called to clean up resources, including the HttpClient instance.
        /// Disposing of resources helps prevent memory leaks and ensures proper cleanup 
        /// of resources when tests are completed.
        /// </summary>
        public void Dispose()
        {
            // Dispose of the HttpClient instance used during tests.
            _client.Dispose();

            // Suppress finalization to prevent garbage collection from calling the destructor.
            GC.SuppressFinalize(this);
        }
    }
}
