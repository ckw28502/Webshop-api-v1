using Microsoft.EntityFrameworkCore.Storage;
using V1.DTOs;
using V1.Models;
using V1.Repositories;
using V1.Utils.EmailSender;
using V1.Utils.PasswordHasher;

namespace V1.Services
{
    /// <summary>
    /// Service responsible for user-related operations
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the UserService class.
    /// </remarks>
    /// <param name="userRepository">The user repository used to interact with user data.</param>
    /// <param name="passwordHasher">The service responsible for securely hashing passwords.</param>
    /// <param name="emailSender">The service responsible for sending emails.</param>
    public class UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IEmailSender emailSender
        ) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IEmailSender _emailSender = emailSender;

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

            // Start transaction
            using IDbContextTransaction transaction = await _userRepository.StartTransaction();

            try
            {
                // Create the user in the repository
                await _userRepository.CreateUser(user);

                // Send a registration confirmation email
                await _emailSender.SendEmail(
                    request.Email,
                    "Confirm registration",
                    "You are registered"
                );

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Rollback the transaction in case of failure
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
