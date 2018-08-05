using System.Collections.Generic;

namespace Bka.TVMazeSraper.Models
{
    /// <summary>
    /// Primary information for a given show with cast
    /// </summary>
    public class TVShow
    {
        /// <summary>
        /// TVMaze unique id
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// Name of the Show.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Last time the show info was updated
        /// </summary>
        public int LastUpdateTime { get; set; }

        public List<ActorTVShow> ActorsTVShows { get; set; }
    }
}
