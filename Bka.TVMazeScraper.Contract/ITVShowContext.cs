using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

using Bka.TVMazeScraper.Models;

namespace Bka.TVMazeScraper.Contracts
{
    public interface ITVShowContext
    {
        DbSet<TVShow> TVShows { get; set; }
        DbSet<Actor> Actors { get; set; }
        /// <summary>
        /// Relation many to many between Actors and TVShows
        /// </summary>
        DbSet<ActorTVShow> ActorsTVShows { get; set; }

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