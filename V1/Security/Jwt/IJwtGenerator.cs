namespace V1.Security.JWT
{
    /// <summary>
    /// Defines a contract for generating JSON Web Tokens (JWT).
    /// </summary>
    public interface IJwtGenerator
    {
        /// <summary>
        /// Generates a JWT access token for the specified user.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="username">The username associated with the user.</param>
        /// <returns>A JWT token as a string.</returns>
        string GenerateToken(Guid id, string username);
    }
}
