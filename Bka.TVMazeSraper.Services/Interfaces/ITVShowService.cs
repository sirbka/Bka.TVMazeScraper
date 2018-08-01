using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bka.TVMazeSraper.Models;

namespace Bka.TVMazeSraper.Services.Interfaces
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
        /// Store list of TV Shows to DataBase, update shows if required
        /// </summary>
        /// <param name="tvShows"></param>
        /// <returns></returns>
        Task<int> StoreTVShows(List<TVShow> tvShows);
    }
}
