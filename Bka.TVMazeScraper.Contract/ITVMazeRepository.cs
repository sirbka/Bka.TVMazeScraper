using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bka.TVMazeScraper.Models;

namespace Bka.TVMazeScraper.Contracts
{
    public interface ITVMazeRepository
    {
        /// <summary>
        /// Scrape specified <count> of TVMaze shows starting from specified <startID>. 
        /// Number of scrapped shows could be lower that <count> if some TV Maze ID are empty
        /// </summary>
        /// <param name="startID">Start TV Maze ID</param>
        /// <param name="count">Number of subsequent TV Maze shows </param>
        /// <param name="cancellationToken"></param>
        Task<List<TVShow>> GetTVMazeShowsWithCast(uint startID = 1, uint count = 20, CancellationToken cancellationToken = default(CancellationToken));
    }
}
