using System.IdentityModel.Tokens.Jwt;

namespace V1.Tests.SecurityTests.JwtTests
{
    /// <summary>
    /// Unit tests for the JWT token generation process.
    /// Verifies that the generated token contains the correct claims and expiration time.
    /// </summary>
    public class GenerateTokenTests : JwtGeneratorTests
    {
        /// <summary>
        /// Tests whether the generated JWT token is valid and contains the correct claims.
        /// </summary>
        [Fact]
        public void GenerateToken_ShouldReturnValidToken()
        {
            // Arrange
            Guid userId = Guid.NewGuid(); // Generate a test user ID
            string username = "test_user"; // Test username
            DateTime current = DateTime.UtcNow; // Capture the current time for validation

            // Act
            string token = _jwtGenerator.GenerateToken(userId, username); // Generate a JWT token

            // Assert
            Assert.NotNull(token); // Ensure the token is not null
            Assert.NotEmpty(token); // Ensure the token is not an empty string
            
            JwtSecurityTokenHandler handler = new(); // Create a handler to read the token
            JwtSecurityToken? jsonToken = handler.ReadToken(token) as JwtSecurityToken; // Parse the token

            Assert.NotNull(jsonToken); // Ensure the token was parsed successfully
            Assert.Equal(_settings.Issuer, jsonToken.Issuer); // Verify the token issuer
            Assert.Contains(_settings.Audience, jsonToken.Audiences); // Verify the token audience
            
            // Verify the user ID claim (subject)
            Assert.Equal(userId.ToString(), jsonToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sub).Value);
            // Verify the username claim
            Assert.Equal(username, jsonToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.UniqueName).Value);
            
            // Verify the expiration time (allowing a small tolerance for time differences)
            Assert.Equal(_settings.ExpiryMinutes, jsonToken.ValidTo.Subtract(current).TotalMinutes, 1);
        }
    }
}
