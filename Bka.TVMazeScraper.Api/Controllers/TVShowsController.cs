﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using AutoMapper;

using Bka.TVMazeScraper.Models;
using Bka.TVMazeScraper.Api.Models;
using Bka.TVMazeScraper.Contracts;

namespace Bka.TVMazeScraper.Api.Controllers
{
    public class TVShowsController : ControllerBase
    {
        private readonly ITVShowService _tvShowService;
        private readonly ILogger<TVShowsController> _logger;
        private readonly IMapper _mapper;

        public TVShowsController(
            ITVShowService tvShowService,
            IMapper mapper,
            ILogger<TVShowsController> logger)
        {
            _tvShowService = tvShowService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Paginated list of all tv shows containing the id of the TV show and a list of 
        /// all the cast that are playing in that TV show ordered by birthday descending.
        /// </summary>
        /// <param name="page">The page, starts at 0</param>
        /// <param name="pagesize">The number of TV shows per page, default is 20</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/shows")]
        public async Task<ICollection<OutputTVShow>> Shows(int page = 0, int pagesize = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (page < 0)
            {
                throw new TVMazeScraperBadRequestException("Page number must be >= 0");
            }

            if ((pagesize < 2) || (page > 200))
            {
                throw new TVMazeScraperBadRequestException("Pagesize number must be between 2 and 200");
            }

            var tvShows = await _tvShowService.GetTVShowsWithCast(page, pagesize, cancellationToken);

            _logger.LogInformation($"Found {tvShows.Count} TV Shows for {page} ({pagesize})");

            var result = _mapper.Map<ICollection<TVShow>, List<OutputTVShow>>(tvShows);

            result.ForEach(s => s.Cast = s.Cast.OrderByDescending(cast => cast.Birthday).ToList());

            return result;
        }
    }
}