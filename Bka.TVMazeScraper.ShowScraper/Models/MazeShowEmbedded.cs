using Newtonsoft.Json;

namespace Bka.TVMazeScraper.ShowScraper.Models
{
    /// <summary>
    /// Is required for successful derealisation af TVMaze data
    /// </summary>
    public class MazeShowEmbedded
    {
        /// <summary>
        /// TVMaze unique id
        /// </summary>
        [JsonProperty(@"id")]
        public uint ID { get; set; }
        /// <summary>
        /// Name of the Show.
        /// </summary>
        [JsonProperty(@"name")]
        public string Name { get; set; }
        /// <summary>
        /// Update hash
        /// </summary>
        [JsonProperty(@"updated")]
        public int LastUpdateTime { get; set; }
        /// <summary>
        /// Embedded collection of Actors
        /// </summary>
        [JsonProperty(@"_embedded")]
        public MazeEmbedded Embedded { get; set; }
    }
}