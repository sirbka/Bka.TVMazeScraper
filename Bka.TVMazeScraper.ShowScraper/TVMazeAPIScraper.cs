using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Bka.TVMazeScraper.Models;
using Bka.TVMazeScraper.ShowScraper.Models;
using Bka.TVMazeScraper.Contracts;
using System.Collections.ObjectModel;

namespace Bka.TVMazeScraper.ShowScraper
{
    /// <summary>
    /// TvMaze API scraper class
    /// </summary>
    public class TVMazeAPIScraper : ITVMazeAPIScraper
    {
        private readonly IHttpClientFactory _httpTVMazeClientFactory;
        private readonly ITVMazeScraperConfiguration _configuration;
        private readonly ILogger<TVMazeAPIScraper> _logger;

        public TVMazeAPIScraper(
            IHttpClientFactory httpClientFactory, 
            ITVMazeScraperConfiguration configuration,
            ILogger<TVMazeAPIScraper> logger)
        {
            _httpTVMazeClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Base TvMaze fetching method
        /// </summary>
        /// <typeparam name="T">Type of Class for the incoming Json to be DeSerialized to.</typeparam>
        /// <param name="link">Relative tvmaze link to data collection</param>
        /// <returns></returns>
        private async Task<T> Scrape<T>(string relativeLink, CancellationToken cancellationToken = default(CancellationToken))
        {
            string responseData = "";

            using (HttpClient client = _httpTVMazeClientFactory.CreateClient(_configuration.TvMazeSrapperHttpClientName))
            {
                HttpResponseMessage response = await client.GetAsync(relativeLink, cancellationToken);
                responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(responseData);
                    }
                    catch (JsonSerializationException ex)
                    {
                        _logger.LogError(ex, $"Error on deserialization of {responseData} ");
                        return default(T);
                    }
                }

                return default(T);
            }
        }

        /// <summary>
        /// Get converted TVMaze Show
        /// </summary>
        /// <param name="TVMazeID">TVMaze Show ID</param>
        /// <returns></returns>
        public async Task<TVShow> GetTVShow(uint TVMazeID, CancellationToken cancellationToken = default(CancellationToken))
        {
            var scrapedShows = await Scrape<MazeShowEmbedded>(string.Format(_configuration.TVMazeShowEmbedCastLinkPostfix, TVMazeID), cancellationToken);

            if (scrapedShows == null)
                return default(TVShow);

            var tvShow = new TVShow()
            {
                ID = scrapedShows.ID,
                Name = scrapedShows.Name,
                LastUpdateTime = scrapedShows.LastUpdateTime
            };

            if (scrapedShows.Embedded?.Cast == null)
                return tvShow;

            var actrShow = tvShow.ActorsTVShows = new Collection<ActorTVShow>();
            foreach (var actor in scrapedShows.Embedded?.Cast)
            {
                var newActor = new Actor()
                {
                    ID = actor.Person.ID,
                    Name = actor.Person.Name,
                    Birthday = actor.Person?.Birthday };

                actrShow.Add(new ActorTVShow()
                {
                    Actor = newActor,
                    ActorID = newActor.ID,
                    TVShow = tvShow,
                    TVShowID = tvShow.ID
                });                        
            }
            
            return tvShow;
        }
    }
}
