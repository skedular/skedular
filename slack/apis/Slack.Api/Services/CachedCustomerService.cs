using Enterprise.Shared.Context;
using Enterprise.Shared.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Slack.Api.Mappers;
using Slack.Shared.Models;
using Slack.Shared.Repositories;

namespace Slack.Api.Services;

public interface ICachedCustomerService
{
    Task<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
    Task<(Customer, Shared.Database.Entities.Customer)> GetAsync(CancellationToken cancellationToken);
    Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(CancellationToken cancellationToken);
    Task<(Customer, Shared.Database.Entities.Customer)> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<(Customer, Shared.Database.Entities.Customer)> GetByVerifiableTokenAsync(
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
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        try
        {
            _ = await GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
            return true;
        }
        catch (CustomerNotFound)
        {
            return false;
        }
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        return await GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
    }

    public async Task<(Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.GetVerifiableToken()))
        {
            return (null, null);
        }

        try
        {
            return await GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return (null, null);
        }
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetByIdAsync(
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

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await memoryCache.GetOrCreateAsync(
            $"customer-verifiabletoken-{context.GetVerifiableToken()}",
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
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        memoryCache.Remove($"customer-verifiabletoken-{context.GetVerifiableToken()}");
    }

    public void CleanCache(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        memoryCache.Remove($"customer-id-{id}");
    }
}
