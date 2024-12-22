using V1.DTOs;

namespace V1.Services
{
    /// <summary>
    /// Service responsible for user-related operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="request">The user data required for registration.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RegisterUser(CreateUserDTO request);
    }
}