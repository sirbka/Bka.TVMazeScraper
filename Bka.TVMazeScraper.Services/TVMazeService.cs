using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Bka.TVMazeScraper.Models;
using Bka.TVMazeScraper.Contracts;

namespace Bka.TVMazeScraper.Services
{
    public class TVMazeService : ITVMazeService
    {
        private readonly ITVMazeRepository _tvMazeRepository;
        private readonly ILogger<TVMazeService> _logger;

        public TVMazeService(ITVMazeRepository tvMazeRepository, ILogger<TVMazeService> logger)
        {
            _tvMazeRepository = tvMazeRepository;
            _logger = logger;
        }

        public async Task<ICollection<TVShow>> Scrape(uint startID = 1, uint count = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _tvMazeRepository.GetTVMazeShowsWithCast(startID, count, cancellationToken);
        }

        public async Task<TVShow> ScrapeById(uint tvMazeID = 1, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tvShows = await _tvMazeRepository.GetTVMazeShowsWithCast(tvMazeID, 1, cancellationToken);

            return tvShows?.FirstOrDefault();
        }
    }
}
