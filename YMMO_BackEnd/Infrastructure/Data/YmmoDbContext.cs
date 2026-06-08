using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data;

public class YmmoDbContext : DbContext
{
    public YmmoDbContext(DbContextOptions<YmmoDbContext> options) : base(options) {}

    // --- DbSets ---
    public DbSet<Agency> Agencies { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Offer> Offers { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- TPH (Table-Per-Hierarchy) for Contact inheritance ---
        // EF Core maps Agent and Client to a single "Contacts" table with a discriminator column
        modelBuilder.Entity<Contact>().ToTable("Contacts")
            .HasDiscriminator<string>("ContactType")
            .HasValue<Agent>("Agent")
            .HasValue<Client>("Client");

        // --- Agency ---
        modelBuilder.Entity<Agency>(entity =>
        {
            entity.HasKey(a => a.AgencyID);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(150);
            entity.Property(a => a.Email).IsRequired().HasMaxLength(200);
            entity.Property(a => a.PhoneNumber).HasMaxLength(20);

            // Agency → Location (N:1)
            entity.HasOne(a => a.Location)
                  .WithMany(l => l.Agencies)
                  .HasForeignKey(a => a.LocationID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Location ---
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(l => l.LocationID);
            entity.Property(l => l.Address).IsRequired().HasMaxLength(250);
            entity.Property(l => l.City).IsRequired().HasMaxLength(100);
            entity.Property(l => l.PostalCode).IsRequired().HasMaxLength(10);
            entity.Property(l => l.Country).IsRequired().HasMaxLength(100);
            entity.Property(l => l.Region).HasMaxLength(100);
            entity.Property(l => l.Complement).HasMaxLength(200);
        });

        // --- Agent ---
        modelBuilder.Entity<Agent>(entity =>
        {
            // Agent → Agency (N:1)
            entity.HasOne(ag => ag.Agency)
                  .WithMany(a => a.Agents)
                  .HasForeignKey(ag => ag.AgencyID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Client ---
        modelBuilder.Entity<Client>(entity =>
        {
            // Client → Agent (N:1, optional)
            entity.HasOne(c => c.LinkedAgent)
                  .WithMany(ag => ag.LinkedClients)
                  .HasForeignKey(c => c.AgentID)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // --- Property ---
        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(p => p.PropertyID);
            entity.Property(p => p.InitialPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.CurrentPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.FinalPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Surface).HasColumnType("decimal(10,2)");
            
            // Ask EFCore to save criteria under type string ("Balcony", "Garage")
            // Instead of type int (0, 3) so it can be Human Readable in the DB
            entity.Property(p => p.Features)
                .HasPostgresArrayConversion(
                    v => v.ToString(),
                    v => (Criteria)Enum.Parse(typeof(Criteria), v));
            
            // Ask EFCore to save PhysicalCondition under type string ("New", "Excellent")
            entity.Property(p => p.Condition)
                .HasConversion<string>();
            
            // Property → Agency (N:1)
            entity.HasOne(p => p.Agency)
                  .WithMany(a => a.Properties)
                  .HasForeignKey(p => p.AgencyID)
                  .OnDelete(DeleteBehavior.Restrict);
            
            // Property → Agent (N:1)
            entity.HasOne(p => p.Agent)
                  .WithMany(a => a.SoldProperties)
                  .HasForeignKey(p => p.AgentID)
                  .OnDelete(DeleteBehavior.Restrict);

            // Property → Location (N:1)
            entity.HasOne(p => p.Location)
                  .WithMany(l => l.Properties)
                  .HasForeignKey(p => p.LocationID)
                  .OnDelete(DeleteBehavior.Restrict);

            // Property → Client Seller (N:1)
            entity.HasOne(p => p.Seller)
                  .WithMany(c => c.OwnedProperties)
                  .HasForeignKey(p => p.SellerID)
                  .OnDelete(DeleteBehavior.Restrict);

            // Property → Client Buyer (N:1, optional)
            entity.HasOne(p => p.Buyer)
                  .WithMany(c => c.BoughtProperties)
                  .HasForeignKey(p => p.BuyerID)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // --- Offer ---
        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(o => o.OfferID);
            entity.Property(o => o.OfferPrice).HasColumnType("decimal(18,2)");

            // Offer → Client (N:1)
            entity.HasOne(o => o.Client)
                  .WithMany(c => c.Offers)
                  .HasForeignKey(o => o.ClientID)
                  .OnDelete(DeleteBehavior.Restrict);

            // Offer → Agent (N:1)
            entity.HasOne(o => o.Agent)
                  .WithMany(ag => ag.ManagedOffers)
                  .HasForeignKey(o => o.AgentID)
                  .OnDelete(DeleteBehavior.Restrict);

            // Offer → Property (N:1)
            entity.HasOne(o => o.Property)
                  .WithMany(p => p.Offers)
                  .HasForeignKey(o => o.PropertyID)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Wishlist ---
        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(w => w.WishlistID);

            // Composite unique constraint: a client can't add the same property twice
            entity.HasIndex(w => new { w.ClientID, w.PropertyID }).IsUnique();

            // Wishlist → Client (N:1)
            entity.HasOne(w => w.Client)
                  .WithMany(c => c.Wishlists)
                  .HasForeignKey(w => w.ClientID)
                  .OnDelete(DeleteBehavior.Cascade);

            // Wishlist → Property (N:1)
            entity.HasOne(w => w.Property)
                  .WithMany(p => p.Wishlists)
                  .HasForeignKey(w => w.PropertyID)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}