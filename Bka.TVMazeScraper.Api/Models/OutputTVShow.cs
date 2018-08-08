using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bka.TVMazeScraper.Api.Models
{
    public class OutputTVShow
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
        /// List of actors
        /// </summary>
        public List<OutputActor> Cast { get; set; }
    }
}
