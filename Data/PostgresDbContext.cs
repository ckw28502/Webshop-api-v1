using Microsoft.EntityFrameworkCore;
using V1.Models;

namespace V1.Data
{
    /// <summary>
    /// Represents the database context for interacting with the PostgreSQL database.
    /// It provides access to the database tables via DbSets.
    /// </summary>
    /// <param name="options">The options to configure the context.</param>
    public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : DbContext(options)
    {

        /// <summary>
        /// Gets or sets the <see cref="DbSet{UserModel}"/> representing the Users table.
        /// </summary>
        public DbSet<UserModel> Users { get; set; } = default!;
    }
}
