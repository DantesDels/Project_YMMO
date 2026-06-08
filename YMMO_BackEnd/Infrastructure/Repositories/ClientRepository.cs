using Microsoft.EntityFrameworkCore;
using YMMO.Backend.Domain.Entities;
using YMMO.Backend.Domain.Repositories;
using YMMO.Backend.Infrastructure.Data;

namespace YMMO.Backend.Infrastructure.Repositories;

public class ClientRepository : BaseRepository<Client>, IClientRepository
{
    public ClientRepository(YmmoDbContext context) : base(context) { }

    public async Task<Client?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(client => client.Email.ToLower() == email.ToLower());
    }

    public async Task<Client?> GetByPhoneNumberAsync(string phoneNumber)
    {
        // PhoneNumber needs format E.164 (ex: +33612345678 | +33 indic for France)
        return await _dbSet
            .FirstOrDefaultAsync(client => client.PhoneNumber == phoneNumber);
    }
}