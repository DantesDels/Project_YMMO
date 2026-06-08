using YMMO.Backend.Domain.Entities;

namespace YMMO.Backend.Domain.Repositories;

public interface IOfferRepository : IBaseRepository<Offer>
{
    Task<IEnumerable<Offer>> GetOffersByPropertyAsync(Guid propertyId);
    Task<IEnumerable<Offer>> GetOffersByClientIdAsync(Guid clientId);
}