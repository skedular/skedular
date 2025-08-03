using Api.Shared.Services;
using Customer.Api.Mappers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Context;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;

namespace Customer.Api.Services;

public interface ICachedCustomerService
{
    Task<bool> DoesCustomerExistAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetAsync(CancellationToken cancellationToken);
    Task<(Shared.Models.Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(CancellationToken cancellationToken);
    Task CleanCacheAsync(Shared.Database.Entities.Customer customer, CancellationToken cancellationToken);
}

public class CachedCustomerService(IRepositoryFactory repositoryFactory, IMapper mapper, IContext context, IMemoryCache memoryCache)
    : ICachedCustomerService
{
    public async Task<bool> DoesCustomerExistAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        try
        {
            _ = await GetByVerifiableTokenAsync(verifiableToken, cancellationToken);
            return true;
        }
        catch (CustomerNotFound)
        {
            return false;
        }
    }

    public async Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        return await GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
    }

    public async Task<(Shared.Models.Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(CancellationToken cancellationToken)
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

    public Task CleanCacheAsync(Shared.Database.Entities.Customer customer, CancellationToken cancellationToken)
    {
        memoryCache.Remove($"customer-id-{customer.Id}");
        foreach (var identity in customer.Identities)
        {
            memoryCache.Remove($"customer-verifiabletoken-{identity.Id}");
        }
        
        return Task.CompletedTask;
    }

    private async Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await memoryCache.GetOrCreateAsync(
            $"customer-verifiabletoken-{context.GetVerifiableToken()}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ??
                               throw new CustomerNotFound();

                return (mapper.MapTo(customer), customer);
            });
    }
}
