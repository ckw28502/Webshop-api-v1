using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace V1.Tests.TestUtilities
{
    /// <summary>
    /// A generic web application factory that allows dynamic injection of service mocks for testing purposes.
    /// This class extends <see cref="WebApplicationFactory{TEntryPoint}"/> to configure test services at runtime.
    /// </summary>
    /// <typeparam name="TService">The type of the service to mock and inject into the service container.</typeparam>
    /// <param name="serviceMock">The mock instance of the service to register.</param>
    public class CustomWebApplicationFactory<TService>(TService serviceMock) : WebApplicationFactory<Program> 
        where TService : class
    {
        /// <summary>
        /// Holds the mock service instance passed to the factory.
        /// </summary>
        private readonly TService _serviceMock = serviceMock;

        /// <summary>
        /// Configures the web host to register the provided mock service in the service collection.
        /// This method is called during the creation of the test server.
        /// </summary>
        /// <param name="builder">The <see cref="IWebHostBuilder"/> to configure.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Call the base configuration to ensure standard web host configurations are applied.
            base.ConfigureWebHost(builder);

            // Inject the mock service into the service collection, replacing any existing service of the same type.
            builder.ConfigureTestServices(services => 
            {
                services.AddSingleton(_serviceMock); // Register the mock instance directly.
            });
        }
    }
}
