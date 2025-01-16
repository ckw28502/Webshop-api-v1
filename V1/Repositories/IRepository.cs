using Microsoft.EntityFrameworkCore.Storage;

namespace V1.Repositories
{
    /// <summary>
    /// Defines the basic contract for repositories with transaction support.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Starts a new database transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing an <see cref="IDbContextTransaction"/>.</returns>
        /// <remarks>
        /// The <see cref="IDbContextTransaction"/> returned by this method can be used to commit or roll back the transaction.
        /// </remarks>
        Task<IDbContextTransaction> StartTransaction();
    }
}
