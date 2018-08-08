using System;

namespace Bka.TVMazeScraper.Contracts
{
    public class TVMazeScraperCustomException : Exception
    {
        public TVMazeScraperCustomException(string message)
        : base(message)
        {
        }

        public TVMazeScraperCustomException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}