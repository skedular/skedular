using System.Text.Json;
using Enterprise.Shared.Outbox;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;
using Location.Shared.Workflows.NewLocationJoined;
using Location.Shared.Workflows.PrecomputeLocationProductRelationships;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Location.Shared.Services;

public class TemporalOutboxExecutorService(ITemporalClient temporalClient) : ITemporalOutboxExecutor
{
    private static readonly string s_generateLocationDailyAnalytics = typeof(GenerateLocationDailyAnalytics).ToWorkflowType();

    private static readonly string s_computeOrganizationLocationsAndProductsRelationships =
        typeof(ComputeOrganizationLocationsAndProductsRelationships).ToWorkflowType();

    private static readonly string s_newLocationJoined = typeof(NewLocationJoined).ToWorkflowType();

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

                _ = await temporalClient.StartWorkflowAsync(
                    (GenerateLocationDailyAnalytics workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_computeOrganizationLocationsAndProductsRelationships)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<ComputeOrganizationLocationsAndProductsRelationshipsInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (ComputeOrganizationLocationsAndProductsRelationships workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_newLocationJoined)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<NewLocationJoinedInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((NewLocationJoined workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }
}
