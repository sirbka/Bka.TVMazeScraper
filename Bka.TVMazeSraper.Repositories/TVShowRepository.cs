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
                using (var transaction = _showContext.BeginTransaction())
                {
                    try
                    {
                        var storedShow = await _showContext.TVShows.FirstOrDefaultAsync(sh => sh.ID == show.ID).ConfigureAwait(false);

                        if (storedShow == null)
                        {
                            _logger.LogInformation( $"Add new TV Show {show.ID} {show.Name} to DB.");
                            var newTVShow = new TVShow()
                            {
                                ID = show.ID,
                                Name = show.Name,
                                LastUpdateTime = show.LastUpdateTime
                            };
                            await  _showContext.TVShows.AddAsync(newTVShow);
                            await _showContext.SaveChangesAsync().ConfigureAwait(false);

                            await StoreCast(show.ActorsTVShows, newTVShow);
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

        private async Task StoreCast(List<ActorTVShow> actorsAndShows, TVShow storedShow)
        {
            foreach (var actorShow in actorsAndShows)
            {
                var storedActor = await _showContext.Actors.FirstOrDefaultAsync(act => act.ID == actorShow.ActorID);
                if (storedActor == null)
                {
                    try
                    {
                        _logger.LogInformation($"Add Actor {actorShow.ActorID} {actorShow?.Actor?.Name} to DB.");
                        var newActor = new Actor()
                        {
                            ID = actorShow.ActorID,
                            Name = actorShow.Actor.Name,
                            Birthday = actorShow.Actor?.Birthday
                        };
                        await _showContext.Actors.AddAsync(newActor);
                        await _showContext.SaveChangesAsync().ConfigureAwait(false);
                        _showContext.ActorsTVShows.Add(
                            new ActorTVShow()
                            {
                                Actor = newActor,
                                ActorID = newActor.ID,
                                TVShow = storedShow,
                                TVShowID = storedShow.ID
                            });
                        await _showContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Something bad happend inside transaction during Actor {actorShow.ActorID} update.");
                        throw;
                    }
                }  
                else
                {
                    if (!_showContext.ActorsTVShows.Any(actShow => actShow.TVShowID == storedShow.ID && actShow.ActorID == storedActor.ID))
                    {
                        _showContext.ActorsTVShows.Add(
                                new ActorTVShow()
                                {
                                    Actor = storedActor,
                                    ActorID = storedActor.ID,
                                    TVShow = storedShow,
                                    TVShowID = storedShow.ID
                                });
                        await _showContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }
}