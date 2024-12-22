using Microsoft.EntityFrameworkCore;
using V1.Data;

namespace V1.Tests.Repositories
{
    /// <summary>
    /// Base class for repository tests that sets up an in-memory database context.
    /// This class implements IDisposable to ensure proper cleanup of database resources.
    /// </summary>
    public abstract class RepositoryTests : IDisposable
    {
        /// <summary>
        /// In-memory database context for testing.
        /// </summary>
        protected readonly PostgresDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTests"/> class.
        /// Sets up an in-memory database for unit tests.
        /// </summary>
        public RepositoryTests()
        {
            DbContextOptions<PostgresDbContext> options = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseInMemoryDatabase("testDatabase") // Use In-Memory DB for testing
                .Options;

            _dbContext = new PostgresDbContext(options);
        }

        /// <summary>
        /// Disposes of the database context and deletes the in-memory database.
        /// Calls GC.SuppressFinalize to prevent redundant finalization.
        /// </summary>
        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();

            // Suppress finalization to avoid unnecessary garbage collection for managed resources.
            GC.SuppressFinalize(this);
        }
    }
}
