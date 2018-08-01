using System.Threading.Tasks;

using Bka.TVMazeSraper.Models;

namespace Bka.TVMazeSraper.ShowScraper.Interfaces
{
    public interface ITVMazeAPIScraper
    {
        Task<TVShow> GeTVShow(uint TVMazeID);
    }
}
