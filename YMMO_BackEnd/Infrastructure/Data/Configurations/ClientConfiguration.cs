using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        // Client → Agent (N:1, optional)
        builder.HasOne(c => c.LinkedAgent)
            .WithMany(ag => ag.LinkedClients)
            .HasForeignKey(c => c.AgentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}