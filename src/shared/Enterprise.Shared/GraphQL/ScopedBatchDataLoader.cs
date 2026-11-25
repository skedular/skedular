using System.Diagnostics;
using System.Diagnostics.Metrics;
using Enterprise.Shared.Metrics;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.GraphQL;

public abstract class ScopedBatchDataLoader<TKey, TValue>(
    IBatchScheduler batchScheduler,
    DataLoaderOptions options,
    IServiceScopeFactory serviceScopeFactory,
    IOpenTelemetryInstrumentation instrumentation)
    : BatchDataLoader<TKey, TValue>(batchScheduler, options)
    where TKey : notnull
{
    private static readonly ActivitySource s_activitySource = new("skedular.graphql.dataloader");
    private readonly Counter<long>? _batchCounter = instrumentation.GetCounterByName<long>(MetricNames.GraphqlDataLoaderBatchCounter);
    private readonly Histogram<double>? _duration = instrumentation.GetHistogramByName<double>(MetricNames.GraphqlDataLoaderDuration);
    private readonly Counter<long>? _keyCounter = instrumentation.GetCounterByName<long>(MetricNames.GraphqlDataLoaderKeyCounter);

    private void RecordBatchSize(string loaderName, int keyCount)
    {
        var tagList = new TagList
        {
            { "graphql.dataloader", loaderName },
        };
        _batchCounter?.Add(1, tagList);
        _keyCounter?.Add(keyCount, tagList);
    }

    protected async Task<TResult> ExecuteInScopeAsync<TService, TResult>(
        IReadOnlyList<TKey> keys,
        Func<TService, Task<TResult>> operation) where TService : notnull
    {
        var loaderName = GetType().Name;
        RecordBatchSize(loaderName, keys.Count);
        var startedAt = Stopwatch.GetTimestamp();
        using var activity = s_activitySource.StartActivity(
            $"graphql.dataloader.{loaderName}",
            ActivityKind.Internal,
            Activity.Current?.Context ?? default(ActivityContext),
            [
                new KeyValuePair<string, object?>("graphql.dataloader", loaderName),
                new KeyValuePair<string, object?>("graphql.dataloader.key_count", keys.Count),
            ]);

        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var result = await operation(scope.ServiceProvider.GetRequiredService<TService>());
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity?.AddException(exception);
            throw;
        }
        finally
        {
            _duration?.Record(Stopwatch.GetElapsedTime(startedAt).TotalSeconds, new TagList
            {
                { "graphql.dataloader", loaderName },
            });
        }
    }
}
