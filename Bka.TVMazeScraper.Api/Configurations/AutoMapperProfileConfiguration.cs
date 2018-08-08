using System.Linq;

using AutoMapper;

using Bka.TVMazeScraper.Api.Models;
using Bka.TVMazeScraper.Models;

namespace Bka.TVMazeScraper.Api.Configurations
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<TVShow, OutputTVShow>()
                .ForMember(destinationMember => destinationMember.Cast,
                    memberOptions => memberOptions.MapFrom(show => show.ActorsTVShows.Select(actorshow => actorshow.Actor)));
            CreateMap<Actor, OutputActor>();
        }
    }
}
