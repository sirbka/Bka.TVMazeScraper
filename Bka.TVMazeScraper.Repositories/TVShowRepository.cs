using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Bka.TVMazeScraper.Models;
using Bka.TVMazeScraper.Contracts;

namespace Bka.TVMazeScraper.Repositories
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

        public async Task<uint> GetLastStoredTVShowID()
        {
            var lastShow = await _showContext.TVShows.LastAsync();
            return lastShow?.ID ?? 0;
        }

        public async Task<List<TVShow>> GetShowsWithCast(int page, int pagesize, CancellationToken cancellationToken)
        {
            return await _showContext.TVShows
                .Include(show => show.ActorsTVShows)
                .ThenInclude(ats => ats.Actor)
                .OrderBy(show => show.ID)
                .Skip(page * pagesize)
                .Take(pagesize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<int> StoreTVShows(List<TVShow> tvShows)
        {
            int processedTVShows = 0; 
            foreach(var show in tvShows)
            {
                processedTVShows++;
                _logger.LogInformation($"Store TV Show {show.ID} - {show.Name} ");

                try
                {
                    var storedShow = await _showContext.TVShows.FirstOrDefaultAsync(sh => sh.ID == show.ID).ConfigureAwait(false);
                    if (storedShow != null)
                        //TODO: Update TVShow if it is required
                        continue;

                    using (var transaction = _showContext.BeginTransaction())
                    {
                        await AddTVShow(show);
                        await StoreCast(show.ActorsTVShows, show);

                        transaction.Commit();                        
                    }
                }
                catch (InvalidOperationException ex)
                {
                    processedTVShows--;
                    _logger.LogError(ex, $"Update exception happened whilst TV Show {show.ID} update.");
                }
                catch (Exception ex)
                {
                    throw new TVMazeScraperCustomException($"Something bad happened whilst TV Show {show.ID} update.", ex);
                }
            }

            return processedTVShows;
        }

        private async Task<TVShow> AddTVShow(TVShow show)
        {
            try
            {
                _logger.LogInformation($"Add new TV Show {show.ID} {show.Name} to DB.");
                var newTVShow = new TVShow()
                {
                    ID = show.ID,
                    Name = show.Name,
                    LastUpdateTime = show.LastUpdateTime
                };

                await _showContext.TVShows.AddAsync(newTVShow);
                await _showContext.SaveChangesAsync().ConfigureAwait(false);

                return newTVShow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something bad happend whilst TVShow {show.ID} update.");
                throw;
            }
        }

        private async Task<Actor> AddActor(Actor actor)
        {
            try
            {
                _logger.LogInformation($"Add Actor {actor.ID} {actor?.Name} to DB.");

                await _showContext.Actors.AddAsync(actor);
                await _showContext.SaveChangesAsync().ConfigureAwait(false);
                return actor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something bad happend whilst Actor {actor.ID} update.");
                throw;
            }
        }

        private async Task<ActorTVShow> AddActorTVShowRelation(Actor actor, TVShow tvShow)
        {
            try
            {
                _logger.LogInformation($"Add Actor ({actor.ID}) TVShow ({tvShow.ID}) relation to DB.");
                var newRelation = new ActorTVShow()
                {
                    Actor = actor,
                    ActorID = actor.ID,
                    TVShow = tvShow,
                    TVShowID = tvShow.ID
                };

                await _showContext.ActorsTVShows.AddAsync(newRelation);
                await _showContext.SaveChangesAsync().ConfigureAwait(false);

                return newRelation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something bad happend whilst Actor ({actor.ID}) TVShow ({tvShow.ID}) relation update.");
                throw;
            }
        }

        private async Task StoreCast(List<ActorTVShow> actorsAndShows, TVShow storedShow)
        {
            foreach (var actorShow in actorsAndShows)
            {
                var storedActor = await _showContext.Actors.FirstOrDefaultAsync(act => act.ID == actorShow.ActorID);
                if (storedActor == null)
                {
                    var newdActor = await AddActor(actorShow.Actor);
                    await AddActorTVShowRelation(newdActor, storedShow);
                }  
                else if (!_showContext.ActorsTVShows.Any(actShow => actShow.TVShowID == storedShow.ID && actShow.ActorID == storedActor.ID))
                {
                    await AddActorTVShowRelation(actorShow.Actor, storedShow);                       
                }
                
            }
        }
    }
}