using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(w => w.WishlistID);

        // Composite unique constraint: a client can't add the same property twice
        builder.HasIndex(w => new { w.ClientID, w.PropertyID }).IsUnique();

        // Wishlist → Client (N:1)
        builder.HasOne(w => w.Client)
            .WithMany(c => c.Wishlists)
            .HasForeignKey(w => w.ClientID)
            .OnDelete(DeleteBehavior.Cascade);

        // Wishlist → Property (N:1)
        builder.HasOne(w => w.Property)
            .WithMany(p => p.Wishlists)
            .HasForeignKey(w => w.PropertyID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}