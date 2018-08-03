using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using Bka.TVMazeSraper.ShowScraper.Models;
using Bka.TVMazeSraper.Models;
using Bka.TVMazeSraper.ShowScraper.Interfaces;

namespace Bka.TVMazeSraper.ShowScraper
{
    /// <summary>
    /// TvMaze API scraper class
    /// </summary>
    public class TVMazeAPIScraper : ITVMazeAPIScraper
    {
        /// <summary>
        /// Show's main information and its cast list in one single response. 
        /// </summary>
        private readonly string _tvMazeShowEmbedCastLink;
        //private readonly IHttpClientFactory _httpClientFactory;
        HttpClient _httpClient;

        public TVMazeAPIScraper(string tvMazeShowEmbedCastLink)//, IHttpClientFactory httpClientFactory)
        {            
            _tvMazeShowEmbedCastLink = tvMazeShowEmbedCastLink;
            //_httpClientFactory = httpClientFactory;
        }

        public TVMazeAPIScraper(HttpClient _client)//, IHttpClientFactory httpClientFactory)
        {
            _httpClient = _client;
            //_httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Base TvMaze fetching method
        /// </summary>
        /// <typeparam name="T">Type of Class for the incoming Json to be DeSerialized to.</typeparam>
        /// <param name="link">Link to data collection</param>
        /// <returns></returns>
        private async Task<T> Scrape<T>(Uri link)
        {
            string results = "";
            string responseData = "";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(link);
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
        /// Get TVMaze Show information and cast by TVMaze ID in one request
        /// </summary>
        /// <param name="TVMazeID">TVMaze Show ID</param>
        /// <returns></returns>
        internal async Task<MazeShowEmbedded> GetShowEmbeddedCast(uint TVMazeID)
        {
            return await Scrape<MazeShowEmbedded>(new Uri(string.Format(_tvMazeShowEmbedCastLink, TVMazeID)));
        }

        /// <summary>
        /// Get converted TVMaze Show
        /// </summary>
        /// <param name="TVMazeID">TVMaze Show ID</param>
        /// <returns></returns>
        public async Task<TVShow> GeTVShow(uint TVMazeID)
        {
            var scrapedShows = await Scrape<MazeShowEmbedded>(new Uri(string.Format(_tvMazeShowEmbedCastLink, TVMazeID)));

            if (scrapedShows != null)
            {
                // TODO: make better converter
                var tvShow = new TVShow() { ID = scrapedShows.ID, Name = scrapedShows.Name, LastUpdateTime = scrapedShows.LastUpdateTime };
                if (scrapedShows.Embedded?.Cast != null)
                {
                    tvShow.Cast = new List<Actor>();
                    foreach (var actor in scrapedShows.Embedded?.Cast)
                    {
                        tvShow.Cast.Add(new Actor() { ID = actor.Person.ID, Name = actor.Person.Name, Birthday = actor.Person.Birthday });
                    }
                }
                return tvShow;
            }

            return default(TVShow);
        }
    }
}
