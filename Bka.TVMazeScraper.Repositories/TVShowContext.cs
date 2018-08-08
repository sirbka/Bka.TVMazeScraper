using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using Bka.TVMazeScraper.Contracts;
using Bka.TVMazeScraper.Models;

namespace Bka.TVMazeScraper.Repositories
{
    public class TVShowContext : DbContext, ITVShowContext
    {
        public TVShowContext(DbContextOptions<TVShowContext> options)
           : base(options)
        {
        }

        public DbSet<TVShow> TVShows { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<ActorTVShow> ActorsTVShows { get; set; }

        public IDbContextTransaction BeginTransaction()
        {
            return Database.BeginTransaction();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActorTVShow>()
                .HasKey(t => new { t.ActorID, t.TVShowID });

            modelBuilder.Entity<ActorTVShow>()
                .HasOne(pt => pt.Actor)
                .WithMany(p => p.ActorsTVShows)
                .HasForeignKey(pt => pt.ActorID);

            modelBuilder.Entity<ActorTVShow>()
                .HasOne(pt => pt.TVShow)
                .WithMany(t => t.ActorsTVShows)
                .HasForeignKey(pt => pt.TVShowID);
        }
    }
}
