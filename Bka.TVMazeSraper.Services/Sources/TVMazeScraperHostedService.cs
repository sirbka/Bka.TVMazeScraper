using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Bka.TVMazeSraper.Services.Interfaces;
using Bka.TVMazeSraper.Models.Interfaces;
using System.IO;

namespace Bka.TVMazeSraper.Services
{
    public class TVMazeScraperHostedService : IHostedService, IDisposable
    {
        private Timer _timer;
        private object _lock = new object();
        private bool firstIteration = true;
        private readonly ITVShowService _tvShowService;
        private readonly ITVMazeService _tvMazeService;
        ITVMazeScraperConfiguration _configuration;
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
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ScrapeAndUpdate, null, TimeSpan.Zero, _configuration.ScraperHostedServiceRepetition);
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
            if (Monitor.TryEnter(_lock))
            {
                _logger.LogInformation("Scrape And Update session started");

                try
                {
                    uint lastShowID = 1;

                    // TODO: enhance update proces: check only updates after all shows were added
                    if (!firstIteration)
                    {
                        lastShowID = await _tvShowService.GetLastStoredTVShowID();
                    }
                    else
                    {
                        firstIteration = false;
                    }

                    await ScrapeFrom(lastShowID);
                    _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(10));
                }
                finally
                {
                    Monitor.Exit(_lock);
                    _logger.LogInformation("Scrape And Update session finished");
                }
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
