using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Bka.TVMazeScraper.Models;
using Bka.TVMazeScraper.Contracts;

namespace Bka.TVMazeScraper.Services
{
    public class TVShowService : ITVShowService
    {
        private readonly ITVShowRepository _tvShowRepository;
        private readonly ILogger<TVShowService> _logger;

        public TVShowService(ITVShowRepository tvshowRepository, ILogger<TVShowService> logger)
        {
            _tvShowRepository = tvshowRepository;
            _logger = logger;            
        }

        public async Task<List<TVShow>> GetTVShowsWithCast(int page, int pagesize, CancellationToken cancellationToken)
        {
            try
            {
                return await _tvShowRepository.GetShowsWithCast(page, pagesize, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new TVMazeScraperCustomException($"Failure to get TV Shows page {page}", ex);
            }
        }

        public async Task<uint> GetLastStoredTVShowID()
        {
            try
            {
                return await _tvShowRepository.GetLastStoredTVShowID();
            }
            catch (Exception ex)
            {
                throw new TVMazeScraperCustomException($"Failure to get last TV Shows ID", ex);
            }
        }

        public async Task<int> StoreTVShows(List<TVShow> tvShows)
        {
            try
            {
                var processedShowsCount = await _tvShowRepository.StoreTVShows(tvShows).ConfigureAwait(false);
                
                _logger.Log( ((tvShows?.Count != processedShowsCount) ? LogLevel.Warning : LogLevel.Information),
                    $" {processedShowsCount} of {tvShows?.Count} TV Shows were processed");

                return processedShowsCount;
            }
            catch (Exception ex)
            {
                throw new TVMazeScraperCustomException($"Failure to store {tvShows?.Count} TV Shows ", ex);
            }
        }
    }
}