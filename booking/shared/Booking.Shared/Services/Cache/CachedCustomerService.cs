using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Microsoft.Extensions.Caching.Hybrid;

namespace Booking.Shared.Services.Cache;

public interface ICachedCustomerService
{
    ValueTask<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
    ValueTask<Customer> GetAsync(CancellationToken cancellationToken);
    ValueTask<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask RemoveAsync(ICollection<Customer> customers, CancellationToken cancellationToken);
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

    public async ValueTask<Customer> GetAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ?? throw new CustomerNotFound();
    }

    public async ValueTask<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.CustomerRepository.GetByIdUntrackedAsync(id, true, ct) ?? throw new CustomerNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return null;
        }
    }

    public async ValueTask RemoveAsync(ICollection<Customer> customers, CancellationToken cancellationToken)
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

    public async ValueTask<Customer?> GetNullableAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        if (string.IsNullOrWhiteSpace(verifiableToken))
        {
            return null;
        }

        return await GetByVerifiableTokenAsync(verifiableToken, cancellationToken) ?? throw new CustomerNotFound();
    }

    public async ValueTask UpdateByIdAsync(string id, CancellationToken cancellationToken)
    {
        await RemoveByIdAsync(id, cancellationToken);

        await hybridCache.SetAsync(
            CreateKeyById(id),
            await repositoryFactory.CustomerRepository.GetByIdUntrackedAsync(id, true, cancellationToken) ?? throw new CustomerNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);
    }

    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    public async ValueTask<Customer?> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyByVerifiableToken(verifiableToken),
                async ct => await repositoryFactory.CustomerRepository.GetByVerifiableTokenUntrackedAsync(verifiableToken, true, ct) ??
                            throw new CustomerNotFound(),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
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
            await repositoryFactory.CustomerRepository.GetByVerifiableTokenUntrackedAsync(verifiableToken, true, cancellationToken) ??
            throw new CustomerNotFound(),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30), LocalCacheExpiration = TimeSpan.FromSeconds(30) },
            cancellationToken: cancellationToken);
    }

    public async ValueTask UpdateAsync(ICollection<Customer> customers, CancellationToken cancellationToken)
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

    public async ValueTask RemoveByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyByVerifiableToken(verifiableToken), cancellationToken);

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-id:{id}";

    private string CreateKeyByVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-verifiabletoken:{verifiableToken}";

    private string CreateKeyByAnyVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-exists-verifiabletoken:{verifiableToken}";
}
