namespace V1.Security.JWT
{
    /// <summary>
    /// Configuration settings for generating and validating JSON Web Tokens (JWT).
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// The secret key used for signing and verifying JWTs. 
        /// Must be at least 256 bits (32 characters) for HMAC-SHA256.
        /// </summary>
        public required string Secret { get; set; }

        /// <summary>
        /// The issuer (authority) of the JWT, typically the server generating the token.
        /// </summary>
        public required string Issuer { get; set; }

        /// <summary>
        /// The intended audience for the JWT, typically the client or API consuming the token.
        /// </summary>
        public required string Audience { get; set; }

        /// <summary>
        /// The duration (in minutes) for which the JWT remains valid before expiration.
        /// Default is 0 if not explicitly set.
        /// </summary>
        public int ExpiryMinutes { get; set; }
    }
}
