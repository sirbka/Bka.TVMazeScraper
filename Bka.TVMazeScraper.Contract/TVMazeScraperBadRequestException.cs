using System;

namespace Bka.TVMazeScraper.Contracts
{
    public class TVMazeScraperBadRequestException : Exception
    {
        public TVMazeScraperBadRequestException()
        {
        }

        public TVMazeScraperBadRequestException(string message)
        : base(message)
        {
        }

        public TVMazeScraperBadRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}