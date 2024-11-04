using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Organization.Api.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface ICachedCustomerService
{
    Task<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);

    Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableCustomerAsync(
        CancellationToken cancellationToken);

    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerByIdAsync(
        string id,
        CancellationToken cancellationToken);

    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken);

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

        try
        {
            _ = await GetCustomerByVerifiableTokenAsync(context.PropertyBag.VerifiableToken, cancellationToken);
            return true;
        }
        catch (CustomerNotFound)
        {
            return false;
        }
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        return await GetCustomerByVerifiableTokenAsync(context.PropertyBag.VerifiableToken, cancellationToken);
    }

    public async Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableCustomerAsync(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.PropertyBag.VerifiableToken))
        {
            return (null, null);
        }

        try
        {
            return await GetCustomerByVerifiableTokenAsync(context.PropertyBag.VerifiableToken, cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return (null, null);
        }
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerByIdAsync(
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

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerByVerifiableTokenAsync(
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
}
