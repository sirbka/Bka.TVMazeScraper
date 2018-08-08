using System;
using System.Globalization;

using Newtonsoft.Json;

namespace Bka.TVMazeScraper.Api.Models
{
    public class OutputActor
    {
        /// <summary>
        /// TVMaze unique id
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// Name of the Actor.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Birthday date
        /// </summary>
        [JsonIgnore]
        public DateTime? Birthday { get; set; }

        [JsonProperty(@"birthday")]
        public string BirthDayOut
        {
            get => Birthday?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "-";
        }
    }
}
