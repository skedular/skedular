using System.Text.Json;
using Enterprise.Shared.Outbox;
using Team.Shared.Workflows.InviteToJoinTeamExistingCustomer;
using Team.Shared.Workflows.InviteToJoinTeamNewCustomer;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Team.Shared.Services;

public class TemporalOutboxExecutorService(ITemporalClient temporalClient) : ITemporalOutboxExecutor
{
    private static readonly string s_inviteToJoinTeamExistingCustomer = typeof(InviteToJoinTeamExistingCustomer).ToWorkflowType();
    private static readonly string s_inviteToJoinTeamNewCustomer = typeof(InviteToJoinTeamNewCustomer).ToWorkflowType();

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_inviteToJoinTeamExistingCustomer)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<InviteToJoinTeamExistingCustomerInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (InviteToJoinTeamExistingCustomer workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_inviteToJoinTeamNewCustomer)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<InviteToJoinTeamNewCustomerInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((InviteToJoinTeamNewCustomer workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }
}
