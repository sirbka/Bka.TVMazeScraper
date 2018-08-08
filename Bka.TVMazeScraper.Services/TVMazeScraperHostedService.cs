using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Bka.TVMazeScraper.Contracts;

namespace Bka.TVMazeScraper.Services
{
    public class TVMazeScraperHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private bool firstIteration = true;
        private TimeSpan _timerDueTime;
        private readonly ITVShowService _tvShowService;
        private readonly ITVMazeService _tvMazeService;
        private readonly ITVMazeScraperConfiguration _configuration;
        private readonly ILogger<TVMazeScraperHostedService> _logger;        

        public TVMazeScraperHostedService(
            ITVShowService tvShowService,
            ITVMazeService tvMazeService,
            ITVMazeScraperConfiguration configuration,
            ILogger<TVMazeScraperHostedService> logger)
        {
            _tvShowService = tvShowService;
            _tvMazeService = tvMazeService;
            _configuration = configuration;
            _timerDueTime = TimeSpan.FromSeconds(_configuration.ScraperHostedServiceRepetitionSeconds);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {

            _timer = new Timer(ScrapeAndUpdate, null, TimeSpan.Zero, _timerDueTime);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async void ScrapeAndUpdate(object state)
        {
            _logger.LogInformation("Scrape And Update session started");
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            try
            {
                uint lastShowID = 1;
                
                // First iteration is always full check
                if (!firstIteration)
                {
                    lastShowID = await _tvShowService.GetLastStoredTVShowID();
                }
                else
                {
                    firstIteration = false;
                }

                // TODO: enhance update proces: check only updates after all shows were added
                await ScrapeFrom(lastShowID);                
            }
            finally
            {
                _logger.LogInformation("Scrape And Update session finished");
                _timer.Change(TimeSpan.Zero, _timerDueTime);
            }
        }

        private async Task<int> ScrapeFrom(uint tvMazeID)
        {
            int updatedTVShows = 0;
            uint pageSize = 20;
            int storedShowsCount = 0;

            do
            {
                var scrappedShows = await _tvMazeService.Scrape(tvMazeID, pageSize);
                if (scrappedShows != null && scrappedShows.Count > 0)
                    storedShowsCount = await _tvShowService.StoreTVShows(scrappedShows);
                
                _logger.Log(LogLevel.Information, $"{scrappedShows?.Count} of {storedShowsCount} scrapped TV Maze Shows were stored of {pageSize}");
                tvMazeID += pageSize;
            }
            while (storedShowsCount > 0);

            return updatedTVShows;
        }
    }
}
