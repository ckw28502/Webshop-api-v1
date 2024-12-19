using V1.DTOs;
using V1.Models;
using V1.Repositories;

namespace V1.Services
{
    /// <summary>
    /// Service responsible for user-related operations
    /// </summary>
    /// <param name="userRepository">The user repository used to interact with user data.</param>
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        /// <summary>
        /// Registers a new user asynchronously, after checking if the username is available.
        /// </summary>
        /// <param name="createUserDto">The data transfer object containing the user's registration information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown if the username is already taken.</exception>
        public async Task RegisterUser(CreateUserDTO request)
        {
            // Check if the username already exists
            if (await _userRepository.UsernameExists(request.Username))
            {
                throw new Exception("USER_USERNAME_EXISTS");
            }

            // Create a new user model from the dto
            var user = new UserModel
            {
                Username = request.Username,
                Password = request.Password
            };

            // Create the user in the repository
            await _userRepository.CreateUser(user);
        }
    }
}