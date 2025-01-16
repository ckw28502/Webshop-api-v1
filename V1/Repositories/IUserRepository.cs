using V1.Models;

namespace V1.Repositories
{
    /// <summary>
    /// Defines the contract for the User repository, including operations
    /// related to user data management.
    /// </summary>
    public interface IUserRepository : IRepository
    {
        /// <summary>
        /// Checks if the given username already exists in the system.
        /// </summary>
        /// <param name="username">The username to check for availability.</param>
        /// <returns>A task representing the asynchronous operation. The task result is a boolean indicating if the username exists.</returns>
        Task<bool> UsernameExists(string username);

        /// <summary>
        /// Checks if the given email already exists in the system.
        /// </summary>
        /// <param name="email">The email to check for availability.</param>
        /// <returns>
        /// A task representing the asynchronous operation. 
        /// The task result is a boolean indicating whether the email exists.
        /// </returns>
        Task<bool> EmailExists(string email);

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="user">The user object to be created.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CreateUser(UserModel user);
    }
}
