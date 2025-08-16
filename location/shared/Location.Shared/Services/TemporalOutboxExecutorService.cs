using System.Text.Json;
using Enterprise.Shared.Outbox;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Location.Shared.Services;

public class TemporalOutboxExecutorService(ITemporalClient temporalClient) : ITemporalOutboxExecutor
{
    private static readonly string s_generateLocationDailyAnalytics = typeof(GenerateLocationDailyAnalytics).ToWorkflowType();

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_generateLocationDailyAnalytics)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<GenerateLocationDailyAnalyticsInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((GenerateLocationDailyAnalytics workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }
}
