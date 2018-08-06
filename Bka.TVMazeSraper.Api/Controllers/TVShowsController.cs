using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using AutoMapper;

using Bka.TVMazeSraper.Services.Interfaces;
using Bka.TVMazeSraper.Models;
using Bka.TVMazeSraper.Api.Map;

namespace Bka.TVMazeSraper.Api.Controllers
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
        public async Task<List<OutputTVShow>> Shows(int page = 0, int pagesize = 20, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (page < 0)
            {
                page = 0;
            }

            if (pagesize < 2)
            {
                pagesize = 2;
            }

            var tvShows = await _tvShowService.GetTVShowsWithCast(page, pagesize, cancellationToken).ConfigureAwait(false);

            _logger.Log(LogLevel.Information, $"Found {tvShows.Count} TV Shows for {page} ({pagesize})");

            var result = _mapper.Map<List<TVShow>, List<OutputTVShow>>(tvShows).ToList();

            result.ForEach(s => s.Cast = s.Cast.OrderByDescending(cast => cast.Birthday).ToList());

            return result;
        }
    }
}