using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bka.TVMazeSraper.Models;

namespace Bka.TVMazeSraper.Services.Interfaces
{
    public interface ITVMazeService
    {
        /// <summary>
        /// Get TVMaze Show with specified ID 
        /// </summary>
        /// <param name="tvMazeID">TV Maze Show ID</param>
        /// <param name="cancellationToken"></param>
        Task<TVShow> ScrapeById(uint tvMazeID = 1, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get specified <count> of TVMaze shows starting from specified <startID>. 
        /// Number of scrapped shows could be lower that <count> if some TV Maze ID are empty
        /// </summary>
        /// <param name="startID">Start TV Maze ID</param>
        /// <param name="count">Number of subsequent TV Maze shows </param>
        /// <param name="cancellationToken"></param>
        Task<List<TVShow>> Scrape(uint startID = 1, uint count = 20, CancellationToken cancellationToken = default(CancellationToken));
    }
}
