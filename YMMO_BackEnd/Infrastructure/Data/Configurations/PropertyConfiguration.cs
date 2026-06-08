using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Enums; // N'oublie pas d'adapter cet using selon où sont tes Enums

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(p => p.PropertyID);
        builder.Property(p => p.InitialPrice).HasColumnType("decimal(18,2)");
        builder.Property(p => p.CurrentPrice).HasColumnType("decimal(18,2)");
        builder.Property(p => p.FinalPrice).HasColumnType("decimal(18,2)");
        builder.Property(p => p.Surface).HasColumnType("decimal(10,2)");
        
        // Ask EFCore to save criteria under type string ("Balcony", "Garage")
        builder.Property(p => p.Features)
            .HasPostgresArrayConversion(
                v => v.ToString(),
                v => (Criteria)Enum.Parse(typeof(Criteria), v));
        
        // Ask EFCore to save PhysicalCondition under type string ("New", "Excellent")
        builder.Property(p => p.Condition)
            .HasConversion<string>();
        
        // Property → Agency (N:1)
        builder.HasOne(p => p.Agency)
               .WithMany(a => a.Properties)
               .HasForeignKey(p => p.AgencyID)
               .OnDelete(DeleteBehavior.Restrict);
        
        // Property → Agent (N:1)
        builder.HasOne(p => p.Agent)
               .WithMany(a => a.SoldProperties)
               .HasForeignKey(p => p.AgentID)
               .OnDelete(DeleteBehavior.Restrict);

        // Property → Location (N:1)
        builder.HasOne(p => p.Location)
               .WithMany(l => l.Properties)
               .HasForeignKey(p => p.LocationID)
               .OnDelete(DeleteBehavior.Restrict);

        // Property → Client Seller (N:1)
        builder.HasOne(p => p.Seller)
               .WithMany(c => c.OwnedProperties)
               .HasForeignKey(p => p.SellerID)
               .OnDelete(DeleteBehavior.Restrict);

        // Property → Client Buyer (N:1, optional)
        builder.HasOne(p => p.Buyer)
               .WithMany(c => c.BoughtProperties)
               .HasForeignKey(p => p.BuyerID)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}