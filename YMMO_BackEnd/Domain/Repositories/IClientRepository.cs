using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Domain.Repositories;

public interface IClientRepository : IBaseRepository<Client>
{
    Task<Client?> GetByEmailAsync(string email);
    Task<Client?> GetByPhoneNumberAsync(string phoneNumber);
}