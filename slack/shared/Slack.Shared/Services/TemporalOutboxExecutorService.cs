using System.Text.Json;
using Enterprise.Shared.Outbox;
using Slack.Shared.Workflows.NewSlackWorkspaceJoined;
using Slack.Shared.Workflows.ReSyncSlackWorkspace;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Slack.Shared.Services;

public class TemporalOutboxExecutorService(ITemporalClient temporalClient) : ITemporalOutboxExecutor
{
    private static readonly string s_newSlackWorkspaceJoined = typeof(NewSlackWorkspaceJoined).ToWorkflowType();
    private static readonly string s_reSyncSlackWorkspace = typeof(ReSyncSlackWorkspace).ToWorkflowType();

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_newSlackWorkspaceJoined)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<NewSlackWorkspaceJoinedInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((NewSlackWorkspaceJoined workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_reSyncSlackWorkspace)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<ReSyncSlackWorkspaceInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((ReSyncSlackWorkspace workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }
}
