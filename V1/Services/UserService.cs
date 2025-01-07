    using V1.DTOs;
    using V1.Models;
    using V1.Repositories;
    using V1.Utils;

    namespace V1.Services
    {
        /// <summary>
        /// Service responsible for user-related operations
        /// </summary>
        /// <param name="userRepository">The user repository used to interact with user data.</param>
        /// <param name="passwordHasher">The service responsible for securely hashing passwords.</param>
        public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserService
        {
            private readonly IUserRepository _userRepository = userRepository;
            private readonly IPasswordHasher _passwordHasher = passwordHasher;

            /// <summary>
            /// Registers a new user asynchronously, after checking if the username is available.
            /// </summary>
            /// <param name="createUserDto">The data transfer object containing the user's registration information.</param>
            /// <returns>A task representing the asynchronous operation.</returns>
            /// <exception cref="Exception">Thrown if the username is already taken.</exception>
            public async Task RegisterUser(CreateUserDto request)
            {
                // Check if the username already exists
                if (await _userRepository.UsernameExists(request.Username))
                {
                    throw new InvalidOperationException("USERNAME_EXISTS");
                }

                // Check if the email already exists
                if (await _userRepository.EmailExists(request.Email))
                {
                    throw new InvalidOperationException("EMAIL_EXISTS");
                }

                // Hash the password
                (byte[] salt, string hashedPassword) = _passwordHasher.HashPassword(request.Password);

                // Create a new user model from the dto
                UserModel user = new()
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = hashedPassword,
                    Salt = salt
                };

                // Create the user in the repository
                await _userRepository.CreateUser(user);
            }
        }
    }