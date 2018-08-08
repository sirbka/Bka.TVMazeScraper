using System;
using System.Collections.Generic;
using System.Globalization;

namespace Bka.TVMazeScraper.Models
{
    /// <summary>
    /// Primary information for a given actor
    /// </summary>
    public class Actor
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
        public DateTime? Birthday { get; set; }

        public List<ActorTVShow> ActorsTVShows { get; set; }

        public override string ToString()
        {
            return $"{ID} {Name} {Birthday?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}";
        }
    }
}
