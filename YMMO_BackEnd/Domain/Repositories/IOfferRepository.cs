namespace YMMO.Backend.Domain.Repositories;

public interface IOfferRepository : IBaseRepository<Offer>
{
    Task<IEnumerable<Offer>> GetOfferByPropertyAsync(Guid propertyId);
    Task<IEnumerable<Offer>> GetOffersByClientIdAsync(Guid clientId);
}