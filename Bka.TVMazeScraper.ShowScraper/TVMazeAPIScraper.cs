using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using Bka.TVMazeScraper.Models;
using Bka.TVMazeScraper.ShowScraper.Models;
using Bka.TVMazeScraper.Contracts;

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
            string results = "";
            string responseData = "";

            using (HttpClient client = _httpTVMazeClientFactory.CreateClient(_configuration.TvMazeSrapperHttpClientName))
            {
                HttpResponseMessage response = await client.GetAsync(relativeLink, cancellationToken);
                responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // TODO: enhance with RegEx
                    results = responseData.Replace("<b>", "").Replace("</b>", "").Replace("<br>", "").Replace("<br />", "")
                        .Replace("<br/>", "").Replace("<div>", "").Replace("<em>", "").Replace("<em/>", "").Replace("<i>", "")
                        .Replace("</i>", "").Replace("<li>", "").Replace("</li>", "").Replace("<p>", "").Replace("</p>", "")
                        .Replace("<ul>", "").Replace("</ul>", "");

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(results);
                    }
                    catch (JsonSerializationException ex)
                    {
                        _logger.LogError(ex, $"Error on deserialization of {results} ");
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

            var actrShow = tvShow.ActorsTVShows = new List<ActorTVShow>();
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
