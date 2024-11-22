using Enterprise.Shared.Database;
using Location.Shared.Database;
using Address = Location.Shared.Database.Entities.Address;

namespace Location.Shared.Repositories;

public interface IAddressRepository : IRepository<Address>
{
}

internal static class AddressExtensions
{
}

public class AddressRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Address>(dbContext), IAddressRepository
{
}
