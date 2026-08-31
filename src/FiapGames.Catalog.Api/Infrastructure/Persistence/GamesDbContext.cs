using FiapGames.Catalog.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace FiapGames.Catalog.Api.Infrastructure.Persistence;

public sealed class GamesDbContext : DbContext
{
    public const string Schema = "catalog";

    public DbSet<Game> Games => Set<Game>();

    public GamesDbContext(DbContextOptions<GamesDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);

        modelBuilder.Entity<Game>(builder =>
        {
            builder.ToTable("games");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Title).IsRequired().HasMaxLength(200);
            builder.Property(g => g.Genre).IsRequired().HasMaxLength(100);
            builder.Property(g => g.Platform).IsRequired().HasMaxLength(100);
            builder.Property(g => g.Description).HasMaxLength(2000);
            builder.Property(g => g.CoverImageUrl).HasMaxLength(2048);
            builder.Property(g => g.Price).HasColumnType("numeric(10,2)");
        });
    }
}
