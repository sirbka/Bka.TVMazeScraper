using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace Bka.TVMazeSraper.Models.Interfaces
{
    public interface ITVShowContext
    {
        DbSet<TVShow> TVShows { get; set; }
        DbSet<Actor> Actors { get; set; }

        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// Saves the changes synchronously.
        /// </summary>
        int SaveChanges();

        /// <summary>
        /// Saves the changes asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default(CancellationToken));
    }
}