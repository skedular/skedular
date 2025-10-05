using Api.Shared.Services;
using Customer.Shared.Repositories;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Security;
using Microsoft.Extensions.Caching.Hybrid;

namespace Customer.Shared.Services.Cache;

public interface ICachedCustomerService : ICustomerHelper
{
    ValueTask<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
    ValueTask<Database.Entities.Customer> GetAsync(CancellationToken cancellationToken);
    ValueTask<Database.Entities.Customer?> GetNullableAsync(CancellationToken cancellationToken);
    ValueTask<Database.Entities.Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask<Database.Entities.Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    ValueTask UpdateByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    ValueTask UpdateAsync(ICollection<Database.Entities.Customer> customers, CancellationToken cancellationToken);
    ValueTask RemoveByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken);
    ValueTask RemoveAsync(ICollection<Database.Entities.Customer> customers, CancellationToken cancellationToken);
}

public class CachedCustomerService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    IContext context,
    HybridCache hybridCache)
    : ICachedCustomerService
{
    public async ValueTask<bool> DoesCustomerExistAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await GetByVerifiableTokenAsync(verifiableToken, cancellationToken) is not null;
    }

    public async ValueTask<bool> DoesCustomerExistAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await GetByVerifiableTokenAsync(verifiableToken, cancellationToken) is not null;
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
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(7), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken)
    {
        await RemoveByIdAsync(id, cancellationToken);

        await hybridCache.SetAsync(
            CreateKeyById(id),
            await repositoryFactory.CustomerRepository.GetByIdUntrackedAsync(id, cancellationToken) ?? throw new CustomerNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(7), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);
    }

    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    public async ValueTask<Database.Entities.Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyByVerifiableToken(verifiableToken),
                async ct => await repositoryFactory.CustomerRepository.GetByVerifiableTokenUntrackedAsync(verifiableToken, ct) ??
                            throw new CustomerNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(7), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return null;
        }
    }

    public async ValueTask UpdateByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        await RemoveByVerifiableTokenAsync(verifiableToken, cancellationToken);

        await hybridCache.SetAsync(
            CreateKeyByVerifiableToken(verifiableToken),
            await repositoryFactory.CustomerRepository.GetByVerifiableTokenUntrackedAsync(verifiableToken, cancellationToken) ??
            throw new CustomerNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(7), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);
    }

    public async ValueTask UpdateAsync(ICollection<Database.Entities.Customer> customers, CancellationToken cancellationToken)
    {
        foreach (var item in customers)
        {
            await RemoveByIdAsync(item.Id, cancellationToken);

            await hybridCache.SetAsync(
                CreateKeyById(item.Id),
                item,
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(7), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);

            foreach (var identity in item.Identities)
            {
                await RemoveByVerifiableTokenAsync(identity.Id, cancellationToken);

                await hybridCache.SetAsync(
                    CreateKeyByVerifiableToken(identity.Id),
                    item,
                    new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(7), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                    cancellationToken: cancellationToken);
            }
        }
    }

    public async ValueTask RemoveByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyByVerifiableToken(verifiableToken), cancellationToken);

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

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-id:{id}";

    private string CreateKeyByVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-verifiabletoken:{verifiableToken}";
}
