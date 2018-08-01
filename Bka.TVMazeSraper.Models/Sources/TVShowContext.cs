using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Bka.TVMazeSraper.Models.Interfaces;

namespace Bka.TVMazeSraper.Models
{
    public class TVShowContext : DbContext, ITVShowContext
    {
        public TVShowContext(DbContextOptions<TVShowContext> options)
           : base(options)
        {            
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Database.BeginTransaction();
        }

        public DbSet<TVShow> TVShows { get; set; }
        public DbSet<Actor> Actors { get; set; }
    }
}
