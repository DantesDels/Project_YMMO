using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;

namespace YMMO.BackEnd.Application.Interfaces;

public interface IYmmoDbContext
{
    DbSet<Agency> Agencies { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<Agent> Agents { get; }
    DbSet<Client> Clients { get; }
    DbSet<Location> Locations { get; }
    DbSet<Offer> Offers { get; }
    DbSet<Property> Properties { get; }
    DbSet<WishlistItem> Wishlists { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}