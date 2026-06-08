using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data;

public class YmmoDbContext : DbContext
{
    public YmmoDbContext(DbContextOptions<YmmoDbContext> options) : base(options) {}

    // --- DbSets ---
    public DbSet<Agency> Agencies { get; set; }
    public DbSet<Contact> Contacts { get; set; } // Ajouté pour la table racine TPH
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Scans the assembly and applies all IEntityTypeConfiguration<T> automatically
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(YmmoDbContext).Assembly);
    }
}