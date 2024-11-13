using Customer.Api.Mappers;
using Customer.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using Enterprise.Shared.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace Customer.Api.Services;

public interface ICachedCustomerService
{
    Task<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
    Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetAsync(CancellationToken cancellationToken);

    Task<(Shared.Models.Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(
        CancellationToken cancellationToken);

    Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetByIdAsync(
        string id,
        CancellationToken cancellationToken);

    Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken);

    void CleanCache();
    void CleanCache(string id);
    void CleanCache(Shared.Database.Entities.Customer customer);
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

        try
        {
            _ = await GetByVerifiableTokenAsync(context.PropertyBag.VerifiableToken, cancellationToken);
            return true;
        }
        catch (CustomerNotFound)
        {
            return false;
        }
    }

    public async Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetAsync(
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        return await GetByVerifiableTokenAsync(context.PropertyBag.VerifiableToken, cancellationToken);
    }

    public async Task<(Shared.Models.Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.PropertyBag.VerifiableToken))
        {
            return (null, null);
        }

        try
        {
            return await GetByVerifiableTokenAsync(context.PropertyBag.VerifiableToken, cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return (null, null);
        }
    }

    public async Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetByIdAsync(
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

    public async Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await memoryCache.GetOrCreateAsync(
            $"customer-verifiabletoken-{context.PropertyBag.VerifiableToken}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                var customer =
                    await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
                        verifiableToken,
                        cancellationToken);
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

    public void CleanCache(Shared.Database.Entities.Customer customer)
    {
        memoryCache.Remove($"customer-id-{customer.Id}");
        customer.Identities.ForEach(identity =>
        {
            memoryCache.Remove($"customer-verifiabletoken-{identity.Id}");
        });
    }
}
