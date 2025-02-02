using Microsoft.EntityFrameworkCore.Storage;
using V1.DTOs;
using V1.Models;
using V1.Repositories;
using V1.Security.JWT;
using V1.Utils.EmailSender;
using V1.Utils.PasswordHasher;

namespace V1.Services
{
    /// <summary>
    /// Service responsible for handling user-related operations such as registration and authentication.
    /// </summary>
    /// <param name="userRepository">Repository interface for user-related database operations.</param>
    /// <param name="passwordHasher">Utility service for securely hashing and verifying passwords.</param>
    /// <param name="emailSender">Service for sending emails, including verification emails.</param>
    /// <param name="jwtGenerator">Service for generating JWT tokens used in authentication and verification.</param>
    public class UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IEmailSender emailSender,
        IJwtGenerator jwtGenerator
        ) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IJwtGenerator _jwtGenerator = jwtGenerator;

        /// <summary>
        /// Registers a new user asynchronously after verifying username and email uniqueness.
        /// </summary>
        /// <param name="request">The DTO containing the user's registration information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the username or email is already in use.
        /// </exception>
        /// <exception cref="Exception">Thrown if an unexpected error occurs during user creation.</exception>
        public async Task RegisterUser(CreateUserDto request)
        {
            // Validate username availability
            if (await _userRepository.UsernameExists(request.Username))
            {
                throw new InvalidOperationException("USERNAME_EXISTS");
            }

            // Validate email availability
            if (await _userRepository.EmailExists(request.Email))
            {
                throw new InvalidOperationException("EMAIL_EXISTS");
            }

            // Securely hash the user's password
            (byte[] salt, string hashedPassword) = _passwordHasher.HashPassword(request.Password);

            // Create a new user model instance
            UserModel user = new()
            {
                Username = request.Username,
                Email = request.Email,
                Password = hashedPassword,
                Salt = salt
            };

            // Generate an email verification token
            string emailVerificationToken = _jwtGenerator.GenerateToken(user.Id, request.Email);
            user.EmailVerificationToken = emailVerificationToken;

            // Begin database transaction to ensure atomic operations
            using IDbContextTransaction transaction = await _userRepository.StartTransaction();

            try
            {
                // Persist the user entity to the database
                await _userRepository.CreateUser(user);

                // Send an email verification message
                await _emailSender.SendUserVerificationEmail(request.Email, emailVerificationToken);

                // Commit the transaction upon success
                await transaction.CommitAsync();
            }
            catch (Exception)
            {                
                // Rollback the transaction to maintain data consistency
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
