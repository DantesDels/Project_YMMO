using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Repositories;

namespace YMMO.Backend.Infrastructure.Data.Configurations;

public class ContactRepository : BaseRepository<Contact>, IContactRepository
{
    public ContactRepository(YmmoDbContext context) : base(context) { }

    public async Task<Contact?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email == email);
    }
}