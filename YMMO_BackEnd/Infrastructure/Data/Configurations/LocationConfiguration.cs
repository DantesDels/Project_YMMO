using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(l => l.LocationID);
        builder.Property(l => l.Address).IsRequired().HasMaxLength(250);
        builder.Property(l => l.City).IsRequired().HasMaxLength(100);
        builder.Property(l => l.PostalCode).IsRequired().HasMaxLength(10);
        builder.Property(l => l.Country).IsRequired().HasMaxLength(100);
        builder.Property(l => l.Region).HasMaxLength(100);
        builder.Property(l => l.Complement).HasMaxLength(200);
    }
}