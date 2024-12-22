using Microsoft.EntityFrameworkCore;
using V1.Data;
using V1.Models;

namespace V1.Repositories
{
    /// <summary>
    /// Implementation of the <see cref="IUserRepository"/> interface for interacting with user data in the database.
    /// Provides methods for performing CRUD operations related to users.
    /// </summary>
    /// <param name="context">The database context used to interact with the database.</param>
    public class UserRepository(PostgresDbContext context) : IUserRepository
    {
        private readonly PostgresDbContext _context = context;

        /// <summary>
        /// Checks if the given username already exists in the database.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns>A task representing the asynchronous operation. The task result is a boolean indicating if the username exists.</returns>
        public async Task<bool> UsernameExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        /// <summary>
        /// Creates a new user in the database.
        /// Adds the user to the <see cref="DbSet{UserModel}"/> and saves the changes.
        /// </summary>
        /// <param name="user">The user to be created.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task CreateUser(UserModel user)
        {
            // Add the user to the Users DbSet
            await _context.Users.AddAsync(user);
            
            // Save the changes to the database asynchronously
            await _context.SaveChangesAsync();
        }
    }
}
