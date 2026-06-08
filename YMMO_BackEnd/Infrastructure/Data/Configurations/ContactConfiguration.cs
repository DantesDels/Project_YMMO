using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        // --- TPH (Table-Per-Hierarchy) for Contact inheritance ---
        builder.ToTable("Contacts")
            .HasDiscriminator<string>("ContactType")
            .HasValue<Agent>("Agent")
            .HasValue<Client>("Client");
    }
}