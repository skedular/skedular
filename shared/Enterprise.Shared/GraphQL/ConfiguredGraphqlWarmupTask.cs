using HotChocolate.Execution;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.GraphQL;

public class ConfiguredGraphqlWarmupTask(string schemaName, IReadOnlyList<string> warmupQueries, ILogger<ConfiguredGraphqlWarmupTask> logger)
    : IRequestExecutorWarmupTask
{
    public bool ApplyOnlyOnStartup => true;

    public async Task WarmupAsync(IRequestExecutor executor, CancellationToken cancellationToken)
    {
        foreach (var warmupQuery in warmupQueries)
        {
            try
            {
                await executor.ExecuteAsync(OperationRequestBuilder.New().SetDocument(warmupQuery).Build(), cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "GraphQL warmup query failed for schema {SchemaName}. Query: {WarmupQuery}", schemaName, warmupQuery);
            }
        }
    }
}
