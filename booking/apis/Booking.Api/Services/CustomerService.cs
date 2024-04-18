using Booking.Api.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Booking.Api.Services;

public interface ICustomerService
{
    Task<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);

    Task<(Customer, Shared.Database.Entities.Customer)>
        GetCustomerAsync(string id, CancellationToken cancellationToken);
}

public class CustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IContext context,
    IMemoryCache memoryCache) : ICustomerService
{
    public async Task<bool> DoesCustomerExistAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        var key = $"customer-exists-{context.PropertyBag.VerifiableToken}";
        if (memoryCache.Get<bool>(key))
        {
            return true;
        }

        memoryCache.Remove(key);
        return await memoryCache.GetOrCreateAsync(
            key,
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                return await repositoryFactory.CustomerRepository.Query(
                    new Specification<Shared.Database.Entities.Customer>
                    {
                        Criteria = query => !query.DeletedAt.HasValue && query.Identities
                            .Select(identity => identity.Id)
                            .Contains(context.PropertyBag.VerifiableToken)
                    }).AsNoTracking().AnyAsync(cancellationToken);
            });
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        var customer =
            await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
                context.PropertyBag.VerifiableToken,
                cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        return (mapper.MapTo(customer)!, customer);
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        string id,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        return (mapper.MapTo(customer)!, customer);
    }
}
