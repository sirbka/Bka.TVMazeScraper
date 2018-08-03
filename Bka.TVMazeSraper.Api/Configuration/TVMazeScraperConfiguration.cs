using Bka.TVMazeSraper.Models.Interfaces;

namespace Bka.TVMazeSraper.Api.Configuration
{
    public class TVMazeScraperConfiguration : ITVMazeScraperConfiguration
    {
        public string TvMazeHttpClientName { get; internal set; }

        public string TvMazeShowEmbedCastLinkPostfix { get; internal set; }
    }
}
