using System.Collections.Generic;

using Newtonsoft.Json;

namespace Bka.TVMazeSraper.ShowScraper.Models
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
        public List<MazeCastMember> Cast { get; set; }
    }
}
