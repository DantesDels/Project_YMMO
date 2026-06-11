using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class PropertyPictureConfiguration : IEntityTypeConfiguration<PropertyPicture>
{
    public void Configure(EntityTypeBuilder<PropertyPicture> builder)
    {
        builder.HasKey(p => p.PropertyPictureId);

        builder.Property(p => p.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.DisplayOrder)
            .IsRequired();

        builder.Property(p => p.IsMain)
            .IsRequired();

        // PropertyPicture → Property (N:1)
        builder.HasOne(p => p.Property)
            .WithMany(p => p.Pictures)
            .HasForeignKey(p => p.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}