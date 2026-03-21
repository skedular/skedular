using Api.Shared.Services;
using Customer.Shared.Repositories;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Security;
using Microsoft.Extensions.Caching.Hybrid;

namespace Customer.Shared.Services.Cache;

public interface ICachedCustomerService : ICustomerHelper
{
    ValueTask<Database.Entities.Customer> GetAsync(CancellationToken cancellationToken);
    ValueTask<Database.Entities.Customer?> GetNullableAsync(CancellationToken cancellationToken);
    ValueTask<Database.Entities.Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask<Database.Entities.Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    ValueTask UpdateAsync(ICollection<Database.Entities.Customer> customers, CancellationToken cancellationToken);
    ValueTask RemoveAsync(ICollection<Database.Entities.Customer> customers, CancellationToken cancellationToken);
}

public class CachedCustomerService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    IContext context,
    HybridCache hybridCache)
    : ICachedCustomerService
{
    public async ValueTask<bool> DoesCustomerExistAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyByAnyVerifiableToken(verifiableToken),
                async ct => await repositoryFactory.CustomerRepository.AnyByVerifiableTokenUntrackedAsync(verifiableToken, ct)
                    ? true
                    : throw new CustomerNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return false;
        }
    }

    public async ValueTask<Database.Entities.Customer> GetAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ?? throw new CustomerNotFound();
    }

    public async ValueTask<Database.Entities.Customer?> GetNullableAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await GetByVerifiableTokenAsync(verifiableToken, cancellationToken);
    }

    public async ValueTask<Database.Entities.Customer?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.CustomerRepository.GetByIdUntrackedAsync(id, ct) ?? throw new CustomerNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return null;
        }
    }

    public async ValueTask<Database.Entities.Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyByVerifiableToken(verifiableToken),
                async ct => await repositoryFactory.CustomerRepository.GetByVerifiableTokenUntrackedAsync(verifiableToken, ct) ??
                            throw new CustomerNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateAsync(ICollection<Database.Entities.Customer> customers, CancellationToken cancellationToken)
    {
        foreach (var item in customers)
        {
            await RemoveByIdAsync(item.Id, cancellationToken);

            await hybridCache.SetAsync(
                CreateKeyById(item.Id),
                item,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);

            foreach (var identity in item.Identities)
            {
                await RemoveByVerifiableTokenAsync(identity.Id, cancellationToken);

                await hybridCache.SetAsync(
                    CreateKeyByVerifiableToken(identity.Id),
                    item,
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                    cancellationToken: cancellationToken);
            }
        }
    }

    public async ValueTask RemoveAsync(ICollection<Database.Entities.Customer> customers, CancellationToken cancellationToken)
    {
        foreach (var item in customers)
        {
            await RemoveByIdAsync(item.Id, cancellationToken);

            foreach (var identity in item.Identities)
            {
                await RemoveByVerifiableTokenAsync(identity.Id, cancellationToken);
            }
        }
    }

    private async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    private async ValueTask RemoveByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyByVerifiableToken(verifiableToken), cancellationToken);

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-id:{id}";

    private string CreateKeyByVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-verifiabletoken:{verifiableToken}";

    private string CreateKeyByAnyVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-exists-verifiabletoken:{verifiableToken}";
}
