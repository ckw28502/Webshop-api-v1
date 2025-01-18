using Microsoft.EntityFrameworkCore.Storage;
using V1.Data;

namespace V1.Repositories
{
    /// <summary>
    /// Base repository class that provides common functionality for all repositories.
    /// This class implements the <see cref="IRepository"/> interface and provides methods
    /// for starting database transactions using Entity Framework.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Repository"/> class with the provided database context.
    /// </remarks>
    /// <param name="context">The database context to be used by the repository.</param>
    public abstract class Repository(PostgresDbContext context) : IRepository
    {
        /// <summary>
        /// The database context used for interacting with the database.
        /// This context is shared by all repositories that inherit from this class.
        /// </summary>
        protected readonly PostgresDbContext _context = context;

        /// <summary>
        /// Starts a new database transaction.
        /// This method is used to wrap database operations in a transaction for consistency and rollback capabilities.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. 
        /// The task result is an <see cref="IDbContextTransaction"/> instance that represents the transaction.
        /// </returns>
        public Task<IDbContextTransaction> StartTransaction()
        {
            return _context.Database.BeginTransactionAsync();
        }
    }
}
