using Enterprise.Shared.Database;
using Payment.Shared.Database;
using Address = Payment.Shared.Database.Entities.Address;

namespace Payment.Shared.Repositories;

public interface IAddressRepository : IRepository<Address>
{
    void Add(Address address);
    void Update(Address address);
    void Remove(Address address);
}

public class AddressRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, Address>(dbContext, timeProvider), IAddressRepository
{
    public void Add(Address address)
    {
        var now = TimeProvider.GetUtcNow();
        address.CreatedAt = now;
        DbContext.Address.Add(address);
    }

    public void Update(Address address)
    {
        var now = TimeProvider.GetUtcNow();
        address.ModifiedAt = now;
        DbContext.Address.Update(address);
    }

    public void Remove(Address address) => DbContext.Address.Remove(address);
}
