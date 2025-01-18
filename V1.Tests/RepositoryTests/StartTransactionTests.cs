using V1.Data;
using V1.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace V1.Tests.RepositoryTests
{
    /// <summary>
    /// Partial implementation of <see cref="TestRepository"/> for testing purposes.
    /// Inherits from the base <see cref="Repository"/> class to reuse core repository functionality.
    /// This class is designed specifically for unit testing the repository's methods.
    /// </summary>
    internal class TestRepository(PostgresDbContext context) : Repository(context)
    { }

    /// <summary>
    /// Unit tests for the <see cref="StartTransaction"/> method in the <see cref="Repository"/> class.
    /// This test class ensures that transactions are correctly initialized within the database context
    /// and that the repository's transaction functionality behaves as expected.
    /// </summary>
    public class StartTransactionTests
    {
        /// <summary>
        /// The instance of <see cref="TestRepository"/> used for testing the transaction behavior.
        /// This field is initialized in the constructor with a newly created instance of <see cref="TestRepository"/>.
        /// It is used to invoke the <see cref="StartTransaction"/> method and perform assertions on its behavior.
        /// </summary>
        private readonly TestRepository _testRepository;

        /// <summary>
        /// Initializes the test class by configuring an SQLite in-memory database.
        /// This allows for testing the repository's transaction behavior without needing an actual database.
        /// </summary>
        public StartTransactionTests()
        {
            // Configure SQLite in-memory database for testing
            DbContextOptions<PostgresDbContext> options = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseSqlite("Data Source=:memory:") // Use SQLite in-memory database for testing
                .Options;

            // Create an instance of PostgresDbContext with the configured options
            PostgresDbContext dbContext = new(options);

            // Open the connection and ensure the database schema is created for testing
            dbContext.Database.OpenConnection();
            dbContext.Database.EnsureCreated();

            // Initialize the TestRepository with the context
            _testRepository = new TestRepository(dbContext);
        }

        /// <summary>
        /// Tests the <see cref="StartTransaction"/> method to ensure that it correctly returns a transaction object.
        /// This method verifies that a transaction is started successfully and that the returned object is of the correct type.
        /// </summary>
        [Fact]
        public async Task StartTransaction_ShouldReturnATransactionObject()
        {
            // Act: Call StartTransaction to start a new transaction using the repository
            IDbContextTransaction transaction = await _testRepository.StartTransaction();

            // Assert: Verify that the returned transaction is not null and is of the correct type
            Assert.NotNull(transaction); // Ensure that the transaction is not null
            Assert.IsAssignableFrom<IDbContextTransaction>(transaction); // Confirm the type of the transaction object

            // Clean-up: Dispose of the transaction to release resources after the test
            await transaction.DisposeAsync();
        }
    }
}
