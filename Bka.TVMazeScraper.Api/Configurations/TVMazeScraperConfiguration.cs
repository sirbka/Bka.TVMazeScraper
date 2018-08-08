using System.ComponentModel.DataAnnotations;

using Bka.TVMazeScraper.Contracts;

namespace Bka.TVMazeScraper.Api.Configurations
{
    public class TVMazeScraperConfiguration : ITVMazeScraperConfiguration
    {
        [Url]
        public string TVMazeShowEmbedCastLink { get; set; }

        [Url]
        public string TVMazeBaseLink { get; set; }

        [Required]
        public string TVMazeShowEmbedCastLinkPostfix { get; set; }

        [MinLength(1)]
        public string TvMazeSrapperHttpClientName { get; set; }

        [Range(10, int.MaxValue, ErrorMessage = "Minimal repetition time for Scraper HostedService is 10 seconds")]
        public int ScraperHostedServiceRepetitionSeconds { get; set; }
    }
}
