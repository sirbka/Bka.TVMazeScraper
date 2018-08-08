using System.Threading;
using System.Threading.Tasks;

using Bka.TVMazeScraper.Models;

namespace Bka.TVMazeScraper.Contracts
{
    public interface ITVMazeAPIScraper
    {
        Task<TVShow> GetTVShow(uint TVMazeID, CancellationToken cancellationToken = default(CancellationToken));
    }
}
