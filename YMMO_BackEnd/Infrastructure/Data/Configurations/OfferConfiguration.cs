using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        // Primary Key
        builder.HasKey(o => o.OfferID);

        // Price formatting
        builder.Property(o => o.OfferPrice)
            .HasColumnType("decimal(18,2)");

        // Enum conversion for status
        builder.Property(o => o.Status)
            .HasConversion<string>();

        // Offer -> Property (N:1)
        builder.HasOne(o => o.Property)
            .WithMany(p => p.Offers)
            .HasForeignKey(o => o.PropertyID)
            .OnDelete(DeleteBehavior.Cascade); // If property is deleted, offers are removed

        // Offer -> Client (N:1)
        builder.HasOne(o => o.Client)
            .WithMany(c => c.Offers)
            .HasForeignKey(o => o.ClientID)
            .OnDelete(DeleteBehavior.Restrict); // Prevent client deletion if they have active offers
    }
}