using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(p => p.PropertyId);

        builder.Property(p => p.PropertyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.PropertyDescription)
            .HasMaxLength(2000);

        builder.Property(p => p.InitialPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.CurrentPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.FinalPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Surface)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        // --- Enums & Collections ---
        builder.Property(p => p.Features)
            .HasColumnType("text[]");
        
        builder.Property(p => p.Condition)
            .HasConversion<string>();

        // --- Relations 1:N ---
        
        // Property -> Pictures (Cascade Delete: if property is deleted, so are the property pictures)
        builder.HasMany(p => p.Pictures)
               .WithOne(pic => pic.Property)
               .HasForeignKey(pic => pic.PropertyId)
               .OnDelete(DeleteBehavior.Cascade);

        // --- Relations N:1 ---
        
        builder.HasOne(p => p.Agency)
               .WithMany(a => a.Properties)
               .HasForeignKey(p => p.AgencyId)
               .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Agent)
               .WithMany(a => a.Properties)
               .HasForeignKey(p => p.AgentId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Location)
               .WithMany(l => l.Properties)
               .HasForeignKey(p => p.LocationId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Seller)
               .WithMany(c => c.OwnedProperties)
               .HasForeignKey(p => p.SellerId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Buyer)
               .WithMany(c => c.BoughtProperties)
               .HasForeignKey(p => p.BuyerId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}