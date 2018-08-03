using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Bka.TVMazeSraper.Models;
using Bka.TVMazeSraper.Models.Interfaces;
using Bka.TVMazeSraper.Services.Interfaces;

namespace Bka.TVMazeSraper.Services
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
                _logger.LogError(ex, $"Failure to get TV Shows page {page}");
                return new List<TVShow>();
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
                _logger.LogError(ex, $"Failure to get last TV Shows ID");
                return 0;
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
                _logger.LogError(ex, $"Failure to store {tvShows?.Count} TV Shows ");
                return 0;
            }
        }
    }
}