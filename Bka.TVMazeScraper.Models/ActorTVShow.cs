namespace Bka.TVMazeScraper.Models
{
    public class ActorTVShow
    {
        public uint ActorID { get; set; }
        public Actor Actor { get; set; }

        public uint TVShowID { get; set; }
        public TVShow TVShow { get; set; }
    }
}
