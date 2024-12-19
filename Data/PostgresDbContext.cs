using Microsoft.EntityFrameworkCore;
using V1.Models;

namespace V1.Data
{
    // Database context class using the primary constructor
    public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : DbContext(options)
    {

        // DbSet for Users table
        public required DbSet<UserModel> Users { get; set; }
    }
}
