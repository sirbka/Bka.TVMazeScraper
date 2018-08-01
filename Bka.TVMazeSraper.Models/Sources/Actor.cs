using System;

namespace Bka.TVMazeSraper.Models
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
    }
}
