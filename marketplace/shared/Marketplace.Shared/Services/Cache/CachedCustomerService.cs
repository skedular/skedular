using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Marketplace.Shared.Database.Entities;
using Marketplace.Shared.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace Marketplace.Shared.Services.Cache;

public interface ICachedCustomerService
{
    ValueTask<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    ValueTask UpdateByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    ValueTask RemoveByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
}

public class CachedCustomerService(ApplicationConfiguration applicationConfiguration, IRepositoryFactory repositoryFactory, HybridCache hybridCache)
    : ICachedCustomerService
{
    public async ValueTask<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.CustomerRepository.GetByIdAsync(id, ct) ?? throw new CustomerNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.SetAsync(
            CreateKeyById(id),
            await repositoryFactory.CustomerRepository.GetByIdAsync(id, cancellationToken) ?? throw new CustomerNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
            cancellationToken: cancellationToken);

    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    public async ValueTask<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyByVerifiableToken(verifiableToken),
                async ct => await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, ct) ?? throw new CustomerNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await hybridCache.SetAsync(
            CreateKeyByVerifiableToken(verifiableToken),
            await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ?? throw new CustomerNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(1), LocalCacheExpiration = TimeSpan.FromMinutes(1) },
            cancellationToken: cancellationToken);

    public async ValueTask RemoveByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyByVerifiableToken(verifiableToken), cancellationToken);

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-id-{id}";

    private string CreateKeyByVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-verifiabletoken-{verifiableToken}";
}
