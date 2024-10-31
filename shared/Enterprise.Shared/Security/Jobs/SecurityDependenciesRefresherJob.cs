using Enterprise.Shared.Configurations;
using Enterprise.Shared.Security.Token;
using Flurl.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Security.Jobs;

public class SecurityDependenciesRefresherJob(
    ILogger<SecurityDependenciesRefresherJob> logger,
    ApplicationConfiguration applicationConfiguration,
    IMemoryCache memoryCache) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (applicationConfiguration.IdentityProviders.Cognito is null)
        {
            return;
        }

        do
        {
            try
            {
                var jws = await memoryCache.GetOrCreateAsync<Jws>("cognito-public-keys", async cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(15);

                    return await applicationConfiguration.IdentityProviders.Cognito.JwksUri.GetJsonAsync<Jws>(
                        cancellationToken: cancellationToken);
                });

                ArgumentNullException.ThrowIfNull(jws);

                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(SecurityDependenciesRefresherJob));
            }
        } while (true);
    }
}
