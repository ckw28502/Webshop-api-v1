using Moq;
using V1.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Xunit.Sdk;

namespace V1.Tests.Controllers
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly Mock<IUserService> _userServiceMock;

        public UserControllerTests(WebApplicationFactory<Program> factory)
        {
            _userServiceMock = new Mock<IUserService>();

            var factoryWithMocks = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                    services.AddSingleton(_userServiceMock.Object)
                );
            });

            _client = factoryWithMocks.CreateClient();
        }

        public static TheoryData<string, string, string> GetInvalidCreateUserDTOs
        {
            get
            {
                return new TheoryData<string, string, string>
                {
                    { "", "Pass1234", "Username is required." },
                    { "u", "Pass1234", "Username must be between 3 and 50 characters." },
                    { "useruseruseruseruseruseruseruseruseruseruseruseruser", "Pass1234", "Username must be between 3 and 50 characters." },
                    { "user", "", "Password is required." },
                    { "user", "p", "Password must be at least 8 characters long." },
                    { "user", "password", "Password must contain at least one uppercase letter." },
                    { "user", "PASSWORD", "Password must contain at least one lowercase letter." },
                    { "user", "Password", "Password must contain at least one number." },
                };
            }
        }

        [Theory]
        [MemberData(nameof(GetInvalidCreateUserDTOs))]
        public async Task CreateUser_ShouldReturnBadRequest_WhenInvalidDTOIsProvided(
            string username, string password, string expectedErrorMessage
        )
        {
            // Arrange
            object requestBody = new { Username = username, Password = password };

            // Act
            HttpResponseMessage response = await _client.PostAsJsonAsync("/", requestBody);

            // Assert
            Assert.Equal(400, (double)response.StatusCode);

            string errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains(expectedErrorMessage, errorMessage);
        }
    }
}