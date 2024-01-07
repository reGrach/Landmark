using Landmark.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Landmark.Database
{
    public class LandmarkContext : DbContext
    {
        //public LandmarkContext() : base() { }
        
        public LandmarkContext(DbContextOptions<LandmarkContext> options)
            : base(options) { }

        public DbSet<Race> Races { get; set; }

        public DbSet<Participant> Participant { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        //    optionsBuilder
        //    .UseNpgsql("Host=localhost;Database=landmark;Username=admin;Password=admin")
        //    .UseSnakeCaseNamingConvention();
    }
}