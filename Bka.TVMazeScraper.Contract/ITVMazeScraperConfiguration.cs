namespace Bka.TVMazeScraper.Contracts
{
    public interface ITVMazeScraperConfiguration
    {
        string TVMazeShowEmbedCastLink { get; }

        string TVMazeBaseLink { get; }

        string TVMazeShowEmbedCastLinkPostfix { get; }

        string TvMazeSrapperHttpClientName { get; }

        int ScraperHostedServiceRepetitionSeconds { get; }
    }
}
