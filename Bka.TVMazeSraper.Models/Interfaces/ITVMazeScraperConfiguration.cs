using System;

namespace Bka.TVMazeSraper.Models.Interfaces
{
    public interface ITVMazeScraperConfiguration
    {
        string TvMazeHttpClientName { get; }

        string TvMazeShowEmbedCastLinkPostfix { get; }

        TimeSpan ScraperHostedServiceRepetition { get; }
    }
}
