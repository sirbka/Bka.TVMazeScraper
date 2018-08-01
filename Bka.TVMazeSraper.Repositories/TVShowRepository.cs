using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Bka.TVMazeSraper.Models;
using Bka.TVMazeSraper.Models.Interfaces;

namespace Bka.TVMazeSraper.Repositories
{
    public class TVShowRepository : ITVShowRepository
    {
        private readonly ILogger<TVShowRepository> _logger;
        private readonly ITVShowContext _showContext;

        public TVShowRepository(
            ILogger<TVShowRepository> logger,
            ITVShowContext showContext)
        {
            _logger = logger;
            _showContext = showContext;
        }

        public async Task<int> GetShowsCount()
        {
            return await _showContext.TVShows.CountAsync();
        }

        public async Task<List<TVShow>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken)
        {
            return await _showContext.TVShows
                .Include(show => show.Cast)
                .OrderBy(show => show.ID)
                .Skip(page * pagesize)
                .Take(pagesize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> StoreTVShows(List<TVShow> tvShows)
        {
            int processedTVShows = 0; 
            foreach(var show in tvShows)
            {
                processedTVShows++;
                _logger.LogInformation($"Store TV Show {show.ID} - {show.Name} ");
                using (var transaction = _showContext.BeginTransaction())
                {
                    try
                    {
                        var storedShow = await _showContext.TVShows.FirstOrDefaultAsync(sh => sh.ID == show.ID).ConfigureAwait(false);

                        if (storedShow == null)
                        {
                            _showContext.TVShows.Add(show);
                            await _showContext.SaveChangesAsync().ConfigureAwait(false);
                            await StoreCast(show.Cast);
                        }
                        else if (storedShow.LastUpdateTime < show.LastUpdateTime)
                        {
                            storedShow.LastUpdateTime = show.LastUpdateTime;
                            await StoreCast(show.Cast);
                        }
                        await _showContext.SaveChangesAsync().ConfigureAwait(false);
                        transaction.Commit();                        
                    }
                    catch (Exception ex)
                    {
                        processedTVShows--;
                        _logger.LogError(ex, $"Something bad happend inside transaction during TV Show {show.ID} update.");
                    }
                }
            }

            return processedTVShows;
        }

        private async Task StoreCast(List<Actor> actors)
        {
            foreach (var actor in actors)
            {
                var storedActor = await _showContext.Actors.FirstOrDefaultAsync(act => act.ID == actor.ID);

                if (storedActor == null)
                {
                    try
                    {
                        _showContext.Actors.Add(actor);
                        await _showContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Something bad happend inside transaction during Actor {actor.ID} update.");
                    }
                }                
            }
        }
    }
}