using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.GraphQL;

public abstract class ScopedBatchDataLoader<TKey, TValue>(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory)
    : BatchDataLoader<TKey, TValue>(batchScheduler, options)
    where TKey : notnull
{
    protected async Task<TResult> ExecuteInScopeAsync<TService, TResult>(
        Func<TService, Task<TResult>> operation)
        where TService : notnull
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        return await operation(scope.ServiceProvider.GetRequiredService<TService>());
    }
}
