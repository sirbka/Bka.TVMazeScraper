using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bka.TVMazeScraper.Models;

namespace Bka.TVMazeScraper.Contracts
{
    public interface ITVShowRepository
    {
        /// <summary>
        /// Get TV Shows from DB
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ICollection<TVShow>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken);

        /// <summary>
        /// Count of TV Shows in DB
        /// </summary>
        /// <returns></returns>
        Task<int> GetShowsCount();

        /// <summary>
        /// Get last stored TVShow ID
        /// </summary>
        /// <returns></returns>
        Task<uint> GetLastStoredTVShowID();

        /// <summary>
        /// Save TV Shows to DB
        /// </summary>
        /// <param name="tvShows"></param>
        /// <returns></returns>
        Task<int> StoreTVShows(ICollection<TVShow> tvShows);
    }
}
