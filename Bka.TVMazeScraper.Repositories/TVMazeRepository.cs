using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;

using Bka.TVMazeScraper.Models;
using Bka.TVMazeScraper.Contracts;


namespace Bka.TVMazeScraper.Repositories
{
    public class TVMazeRepository : ITVMazeRepository
    {
        private readonly ILogger<TVMazeRepository> _logger;
        private readonly ITVMazeAPIScraper _tvMazeApiScraper;

        public TVMazeRepository(
            ILogger<TVMazeRepository> logger,
            ITVMazeAPIScraper tvMazeApiScraper)
        {
            _logger = logger;
            _tvMazeApiScraper = tvMazeApiScraper;
        }

        public async Task<ICollection<TVShow>> GetTVMazeShowsWithCast(uint startID = 1, uint count = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new Collection<TVShow>();
            for (uint i = startID; i < startID + count; i++)
            {
                var tvShow = await _tvMazeApiScraper.GetTVShow(i, cancellationToken);
                if (tvShow != null)
                    result.Add(tvShow);
            }
            _logger.LogInformation($"{result.Count} of {count} TV Maze Shows were scrapped starting {startID} TV Maze Show ID");
            return result;
        }
    }
}