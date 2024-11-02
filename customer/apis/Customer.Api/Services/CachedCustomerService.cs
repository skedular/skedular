using Customer.Api.Mappers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Customer.Api.Services;

public interface ICachedCustomerService
{
    Task<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);

    Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        CancellationToken cancellationToken);

    Task<(Shared.Models.Customer?, Shared.Database.Entities.Customer?)> GetNullableCustomerAsync(
        CancellationToken cancellationToken);

    Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)>
        GetCustomerAsync(string id, CancellationToken cancellationToken);

    void CleanCache();
    void CleanCache(string id);
}

public class CachedCustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IContext context,
    IMemoryCache memoryCache) : ICachedCustomerService
{
    public async Task<bool> DoesCustomerExistAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        var key = $"customer-exists-{context.PropertyBag.VerifiableToken}";
        if (memoryCache.TryGetValue<bool>(key, out var entry))
        {
            if (entry)
            {
                return true;
            }
        }

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

    public async Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        return await memoryCache.GetOrCreateAsync(
            $"customer-verifiabletoken-{context.PropertyBag.VerifiableToken}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                var customer =
                    await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
                        context.PropertyBag.VerifiableToken,
                        cancellationToken);
                if (customer is null)
                {
                    throw new CustomerNotFound();
                }

                return (mapper.MapTo(customer)!, customer);
            });
    }

    public async Task<(Shared.Models.Customer?, Shared.Database.Entities.Customer?)> GetNullableCustomerAsync(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.PropertyBag.VerifiableToken))
        {
            return (null, null);
        }

        try
        {
            return await GetCustomerAsync(cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return (null, null);
        }
    }

    public async Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        string id,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        return await memoryCache.GetOrCreateAsync(
            $"customer-id-{id}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(id, cancellationToken);
                if (customer is null)
                {
                    throw new CustomerNotFound();
                }

                return (mapper.MapTo(customer)!, customer);
            });
    }

    public void CleanCache()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        memoryCache.Remove($"customer-verifiabletoken-{context.PropertyBag.VerifiableToken}");
    }

    public void CleanCache(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        memoryCache.Remove($"customer-id-{id}");
    }
}
