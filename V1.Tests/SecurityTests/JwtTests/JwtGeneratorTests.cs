using Microsoft.Extensions.Options;
using V1.Security.JWT;

namespace V1.Tests.SecurityTests.JwtTests
{
    /// <summary>
    /// Unit tests for the JwtGenerator class.
    /// This class verifies that JWT generation behaves as expected.
    /// </summary>
    public class JwtGeneratorTests
    {
        /// <summary>
        /// JWT settings used for testing.
        /// </summary>
        protected readonly JwtSettings _settings;

        /// <summary>
        /// Instance of JwtGenerator used in tests.
        /// </summary>
        protected readonly JwtGenerator _jwtGenerator; 

        /// <summary>
        /// Initializes a new instance of <see cref="JwtGeneratorTests"/>.
        /// Sets up the JWT settings and initializes the JwtGenerator.
        /// </summary>
        public JwtGeneratorTests()
        {
            // Initialize JWT settings for testing
            _settings = new()
            {
                Secret = "test_secret_key_for_jwt_generator_test", // Secret key for signing JWTs
                Issuer = "http://localhost", // Issuer of the token
                Audience = "http://localhost", // Expected audience for the token
                ExpiryMinutes = 15 // Token expiration time in minutes
            };

            // Wrap the settings in IOptions to match JwtGenerator constructor
            IOptions<JwtSettings> settings = Options.Create(_settings);

            // Create an instance of JwtGenerator with the test settings
            _jwtGenerator = new JwtGenerator(settings);
        }  
    }
}
