using Microsoft.EntityFrameworkCore;

using Bka.TVMazeScraper.Models;

namespace Bka.TVMazeScraper.Repositories
{
    public class TVShowContext : DbContext
    {
        public TVShowContext(DbContextOptions<TVShowContext> options)
           : base(options)
        {
        }

        public DbSet<TVShow> TVShows { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<ActorTVShow> ActorsTVShows { get; set; }

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
