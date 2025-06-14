using Api.Shared.Services;
using Customer.Api.Mappers;
using Customer.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Context;
using Microsoft.Extensions.Caching.Memory;

namespace Customer.Api.Services;

public interface ICachedCustomerService
{
    Task<bool> DoesCustomerExistAsync(string verifiableToken, CancellationToken cancellationToken);
    Task<(Shared.Models.Customer, Shared.Database.Entities.Customer)> GetAsync(CancellationToken cancellationToken);
    Task<(Shared.Models.Customer?, Shared.Database.Entities.Customer?)> GetNullableAsync(CancellationToken cancellationToken);
    void CleanCache(Shared.Database.Entities.Customer customer);
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

    public void CleanCache(Shared.Database.Entities.Customer customer)
    {
        memoryCache.Remove($"customer-id-{customer.Id}");
        customer.Identities.ForEach(identity => { memoryCache.Remove($"customer-verifiabletoken-{identity.Id}"); });
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

                var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken);
                if (customer is null)
                {
                    throw new CustomerNotFound();
                }

                return (mapper.MapTo(customer), customer);
            });
    }
}
