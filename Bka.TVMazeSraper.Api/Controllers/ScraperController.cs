using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Bka.TVMazeSraper.Services.Interfaces;

namespace Bka.TVMazeSraper.Api.Controllers
{
    public class ScraperController : ControllerBase
    {
        private readonly ITVShowService _tvShowService;
        private readonly ITVMazeService _tvMazeService;
        private readonly ILogger<ScraperController> _logger;

        public ScraperController(
            ITVShowService tvShowService,
            ITVMazeService tvMazeService,
            ILogger<ScraperController> logger)
        {
            _tvShowService = tvShowService;
            _tvMazeService = tvMazeService;
            _logger = logger;
        }

        /// <summary>
        /// Scrape TVMaze Shows specified number of TVMaze shows
        /// </summary>
        /// <param name="start">Start TVMaze ID</param>
        /// <param name="count">The number of TVMaze IDs for processing</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/scrape")]
        public async Task<string> ScrapeAndStore(int start = 1, int count = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (start < 1)
            {
                start = 1;
            }
            if (count < 1)
            {
                count = 1;
            }

            int storedShowsCount = 0;
            var scrappedShows = await _tvMazeService.Scrape((uint)start, (uint)count, cancellationToken).ConfigureAwait(false);
            if (scrappedShows != null && scrappedShows.Count > 0)
                storedShowsCount = await _tvShowService.StoreTVShows(scrappedShows).ConfigureAwait(false);

            var result = $"{scrappedShows?.Count} of {storedShowsCount} scrapped TV Maze Shows were stored of {count}";
            _logger.Log(LogLevel.Information, result);

            return result;
        }
    }
}