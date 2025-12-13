using GuiaGastronomica.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GuiaGastronomica.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Venue> Venues { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<UserFeedback> UserFeedback { get; set; }
    public DbSet<Zone> Zones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuraciones adicionales si es necesario
        modelBuilder.Entity<Venue>()
            .HasIndex(v => v.Zone);

        modelBuilder.Entity<Venue>()
            .HasIndex(v => v.Category);

        modelBuilder.Entity<Venue>()
            .HasIndex(v => v.Score);
    }
}
