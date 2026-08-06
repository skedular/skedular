using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Marketplace.Shared.Database.Entities;
using Marketplace.Shared.Repositories;
using Microsoft.Extensions.Caching.Hybrid;

namespace Marketplace.Shared.Services.Cache;

public interface ICachedCustomerService
{
    ValueTask<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
    ValueTask<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken);
    ValueTask RemoveAsync(IReadOnlyList<Customer> customers, CancellationToken cancellationToken);
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
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromDays(30),
                    LocalCacheExpiration = TimeSpan.FromDays(7),
                },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return false;
        }
    }

    public async ValueTask<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            return await hybridCache.GetOrCreateAsync(
                CreateKeyById(id),
                async ct => await repositoryFactory.CustomerRepository.GetByIdAsync(id, ct) ?? throw new CustomerNotFound(),
                new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30),
                    LocalCacheExpiration = TimeSpan.FromSeconds(30),
                },
                cancellationToken: cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return null;
        }
    }

    public async ValueTask RemoveAsync(IReadOnlyList<Customer> customers, CancellationToken cancellationToken)
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

    public async ValueTask RemoveByIdAsync(string id, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyById(id), cancellationToken);

    private async ValueTask RemoveByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(CreateKeyByVerifiableToken(verifiableToken), cancellationToken);

    private string CreateKeyById(string id) => $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-id:{id}";

    private string CreateKeyByVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-verifiabletoken:{verifiableToken}";

    private string CreateKeyByAnyVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-exists-verifiabletoken:{verifiableToken}";
}
