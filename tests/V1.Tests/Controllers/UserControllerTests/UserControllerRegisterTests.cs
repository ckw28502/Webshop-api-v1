using System.Net;
using System.Net.Http.Json;
using Moq;
using V1.DTOs;
using V1.Services;

namespace V1.Tests.Controllers.UserControllerTests
{
    /// <summary>
    /// Tests for the UserController's Register endpoint.
    /// This class contains tests for different scenarios when registering a user, including validation errors and service failures.
    /// </summary>
    public class UserControllerRegisterTests : ControllerTests<IUserService>
    {
        /// <summary>
        /// A valid CreateUserDto object used for testing successful user creation.
        /// </summary>
        private readonly CreateUserDto _validRequest;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControllerRegisterTests"/> class.
        /// Sets up the test with a valid CreateUserDto request.
        /// </summary>
        public UserControllerRegisterTests() : base()
        {
            _validRequest = new CreateUserDto
            {
                Username = "user",
                Password = "User1234"
            };
        }

        /// <summary>
        /// Provides test data for invalid CreateUserDto inputs.
        /// This is used for testing various validation error scenarios when the user input is incorrect.
        /// </summary>
        public static TheoryData<string, string, string> GetInvalidCreateUserDtos
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

        /// <summary>
        /// Test method that ensures a bad request response is returned when invalid user data is provided.
        /// </summary>
        /// <param name="username">The username input to be tested.</param>
        /// <param name="password">The password input to be tested.</param>
        /// <param name="expectedErrorMessage">The expected error message in the response.</param>
        [Theory]
        [MemberData(nameof(GetInvalidCreateUserDtos))]
        public async Task CreateUser_ShouldReturnBadRequest_WhenInvalidDTOIsProvided(
            string username, string password, string expectedErrorMessage
        )
        {
            // Arrange: Create an object with the invalid username and password to simulate the request.
            object requestBody = new { Username = username, Password = password };

            // Act: Send the POST request to the /user endpoint with the invalid data.
            HttpResponseMessage response = await _client.PostAsJsonAsync("/user", requestBody);

            // Assert: Verify that a BadRequest (400) status code is returned.
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Assert: Check if the response contains the expected error message.
            string errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains(expectedErrorMessage, errorMessage);

            // Assert: Verify that the RegisterUser method was never called.
            _serviceMock.Verify(service => service.RegisterUser(It.IsAny<CreateUserDto>()), Times.Never);
        }

        /// <summary>
        /// Test method to ensure that when the UserService throws an exception during user registration, 
        /// a BadRequest (400) status code is returned, along with the appropriate error message.
        /// </summary>
        [Fact]
        public async Task CreateUser_ShouldReturn_BadRequest_WhenUserServiceThrowsException()
        {
            // Arrange: Set up the service mock to throw an exception when attempting to register a user.
            string expectedErrorMessage = "service error";
            _serviceMock
                .Setup(service => service.RegisterUser(It.IsAny<CreateUserDto>()))
                .ThrowsAsync(new Exception(expectedErrorMessage));

            // Act: Send the POST request with a valid user creation DTO.
            HttpResponseMessage response = await _client.PostAsJsonAsync("/user", _validRequest);

            // Assert: Verify that the response status code is BadRequest (400).
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Assert: Check if the response contains the error message thrown by the service.
            string errorMessage = await response.Content.ReadAsStringAsync();
            Assert.True(errorMessage.Contains("service error"), $"Error message was: {errorMessage}");

            // Assert: Verify that the RegisterUser method was called exactly once.
            _serviceMock.Verify(service => service.RegisterUser(It.IsAny<CreateUserDto>()), Times.Once);
        }

        /// <summary>
        /// Tests the scenario where a valid user is registered successfully.
        /// Verifies that the API returns a Created (201) status code and the user registration is handled properly.
        /// </summary>
        [Fact]
        public async Task CreateUser_ShouldReturn_Created_WhenUserIsCreated()
        {
            // Act: Send the POST request with a valid user creation DTO.
            HttpResponseMessage response = await _client.PostAsJsonAsync("/user", _validRequest);

            // Assert: Verify that the response status code is Created (201).
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Assert: Verify that the RegisterUser method was called exactly once.
            _serviceMock.Verify(service => service.RegisterUser(It.IsAny<CreateUserDto>()), Times.Once);
        }
    }
}
