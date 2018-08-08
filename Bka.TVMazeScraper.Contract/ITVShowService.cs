using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bka.TVMazeScraper.Models;

namespace Bka.TVMazeScraper.Contracts
{
    public interface ITVShowService
    {
        /// <summary>
        /// Get TV Shows with Cast from DataBase
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TVShow>> GetTVShowsWithCast(int page, int pagesize, CancellationToken cancellationToken);

        /// <summary>
        /// Last TVShow ID in Database
        /// </summary>
        Task<uint> GetLastStoredTVShowID();

        /// <summary>
        /// Store list of TV Shows to DataBase, update shows if required
        /// </summary>
        /// <param name="tvShows"></param>
        /// <returns></returns>
        Task<int> StoreTVShows(List<TVShow> tvShows);
    }
}
