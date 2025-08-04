using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Caching.Hybrid;

namespace Api.Shared.Services.Cache;

public interface IGenericCustomerCacheService
{
    ValueTask<T> GetByVerifiableTokenAsync<T>(
        string verifiableToken,
        Func<CancellationToken, ValueTask<T>> factory,
        CancellationToken cancellationToken);

    ValueTask InvalidateByVerifiableTokensAsync(IEnumerable<string> verifiableTokens, CancellationToken cancellationToken);
}

public class GenericCustomerCacheService(ApplicationConfiguration applicationConfiguration, HybridCache hybridCache) : IGenericCustomerCacheService
{
    public async ValueTask<T> GetByVerifiableTokenAsync<T>(
        string verifiableToken,
        Func<CancellationToken, ValueTask<T>> factory,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await hybridCache.GetOrCreateAsync(
            CreateKeyByVerifiableToken(verifiableToken),
            factory,
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromDays(7), LocalCacheExpiration = TimeSpan.FromMilliseconds(1) },
            cancellationToken: cancellationToken);
    }

    public async ValueTask InvalidateByVerifiableTokensAsync(IEnumerable<string> verifiableTokens, CancellationToken cancellationToken) =>
        await hybridCache.RemoveAsync(verifiableTokens.Select(CreateKeyByVerifiableToken), cancellationToken);

    private string CreateKeyByVerifiableToken(string verifiableToken) =>
        $"{applicationConfiguration.Environment}:{applicationConfiguration.Domain}:customer-verifiabletoken-{verifiableToken}";
}
