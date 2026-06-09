using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Domain.Repositories;

public interface IContactRepository
{
    Task<Contact?> GetByIdAsync(Guid contactId);
    Task<Contact?> GetByEmailAsync(string email);
    Task UpdateAsync(Contact contact);
}