using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Address = Payment.Shared.Database.Entities.Address;

namespace Payment.Shared.Repositories;

public interface IAddressRepository : IRepository<Address>
{
    Address Add(Address address);
    Address Update(Address address);
    Address Remove(Address address);
}

public class AddressRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, Address>(dbContext, timeProvider), IAddressRepository
{
    public Address Add(Address address)
    {
        var now = TimeProvider.GetUtcNow();
        address.CreatedAt = now;
        return DbContext.Address.Add(address).Entity;
    }

    public Address Update(Address address)
    {
        var now = TimeProvider.GetUtcNow();
        address.ModifiedAt = now;
        return DbContext.Address.Update(address).Entity;
    }

    public Address Remove(Address address) => DbContext.Address.Remove(address).Entity;
}
