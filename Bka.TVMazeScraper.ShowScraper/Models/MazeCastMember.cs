using Newtonsoft.Json;

namespace Bka.TVMazeScraper.ShowScraper.Models
{
    public class MazeCastMember
    {
        /// <summary>
        /// Primary information for a given person
        /// </summary>
        [JsonProperty(@"person")]
        public MazePerson Person { get; set; }
    }
}