using System;

using Newtonsoft.Json;

namespace Bka.TVMazeSraper.ShowScraper.Models
{
    /// <summary>
    /// Primary information for a given person
    /// </summary>
    public class MazePerson
    {
        /// <summary>
        /// TVMaze unique id
        /// </summary>
        [JsonProperty(@"id")]
        public uint ID { get; set; }
        /// <summary>
        /// Name of the Actor.
        /// </summary>
        [JsonProperty(@"name")]
        public string Name { get; set; }
        /// <summary>
        /// Birthday date
        /// </summary>
        [JsonProperty(@"birthday")]
        public DateTime? Birthday { get; set; }
    }
}