using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bka.TVMazeScraper.ShowScraper.Models
{
    /// <summary>
    /// Is required for successful derealisation af TVMaze data
    /// </summary>
    public class MazeEmbedded
    {
        /// <summary>
        /// Collection of Actors
        /// </summary>
        [JsonProperty(@"cast")]
        public IEnumerable<MazeCastMember> Cast { get; set; }
    }
}
