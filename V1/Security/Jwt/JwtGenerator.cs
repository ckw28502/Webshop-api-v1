using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace V1.Security.JWT
{
    /// <summary>
    /// Generates JSON Web Tokens (JWT) for authentication and authorization.
    /// </summary>
    /// <param name="jwtSettings">The application JWT settings.</param>
    public class JwtGenerator(IOptions<JwtSettings> jwtSettings) : IJwtGenerator
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        /// <summary>
        /// Generates a JWT access token for the specified user.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="username">The username associated with the user.</param>
        /// <returns>A signed JWT token as a string.</returns>
        public string GenerateToken(Guid id, string username)
        {
            // Create a symmetric security key from the secret
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            // Create signing credentials using HMAC SHA-256
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            // Define claims (payload) for the token
            List<Claim> claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()), // Subject (user ID)
                new Claim(JwtRegisteredClaimNames.UniqueName, username), // Unique username
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token identifier
            ];

            // Create a JWT token with issuer, audience, claims, expiration, and signing credentials
            JwtSecurityToken token = new(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            // Generate and return the JWT token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }   
    }
}
