using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Bka.TVMazeSraper.Models;
using Bka.TVMazeSraper.Models.Interfaces;
using Bka.TVMazeSraper.ShowScraper.Interfaces;
using Bka.TVMazeSraper.ShowScraper.Models;

namespace Bka.TVMazeSraper.ShowScraper
{
    /// <summary>
    /// TvMaze API scraper class
    /// </summary>
    public class TVMazeAPIScraper : ITVMazeAPIScraper
    {
        private readonly IHttpClientFactory _httpTVMazeClientFactory;
        private readonly ITVMazeScraperConfiguration _configuration;

        public TVMazeAPIScraper(IHttpClientFactory httpClientFactory, ITVMazeScraperConfiguration configuration)
        {
            _httpTVMazeClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        /// <summary>
        /// Base TvMaze fetching method
        /// </summary>
        /// <typeparam name="T">Type of Class for the incoming Json to be DeSerialized to.</typeparam>
        /// <param name="link">Relative tvmaze link to data collection</param>
        /// <returns></returns>
        private async Task<T> Scrape<T>(string relativeLink)
        {
            string results = "";
            string responseData = "";

            using (HttpClient client = _httpTVMazeClientFactory.CreateClient(_configuration.TvMazeHttpClientName))
            {
                HttpResponseMessage response = await client.GetAsync(relativeLink);
                responseData = await response.Content.ReadAsStringAsync();

                if (response.ReasonPhrase == "OK")
                {
                    results = responseData.Replace("<b>", Environment.NewLine);
                    results = results.Replace("</b>", "");
                    results = results.Replace("<br>", "");
                    results = results.Replace("<br />", "");
                    results = results.Replace("<br/>", "");
                    results = results.Replace("<div>", "");
                    results = results.Replace("<em>", "");
                    results = results.Replace("<em/>", "");
                    results = results.Replace("<i>", "");
                    results = results.Replace("</i>", "");
                    results = results.Replace("<li>", "");
                    results = results.Replace("</li>", "");
                    results = results.Replace("<p>", "");
                    results = results.Replace("</p>", "");
                    results = results.Replace("<ul>", "");
                    results = results.Replace("</ul>", "");

                    try
                    {
                        return JsonConvert.DeserializeObject<T>(results);
                    }
                    catch (JsonSerializationException)
                    {
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
        public async Task<TVShow> GeTVShow(uint TVMazeID)
        {
            var scrapedShows = await Scrape<MazeShowEmbedded>(string.Format(_configuration.TvMazeShowEmbedCastLinkPostfix, TVMazeID));

            if (scrapedShows != null)
            {
                var tvShow = new TVShow() { ID = scrapedShows.ID, Name = scrapedShows.Name, LastUpdateTime = scrapedShows.LastUpdateTime };

                if (scrapedShows.Embedded?.Cast != null)
                {
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
                }
                return tvShow;
            }

            return default(TVShow);
        }
    }
}
