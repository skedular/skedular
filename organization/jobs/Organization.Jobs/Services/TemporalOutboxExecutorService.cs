using System.Text.Json;
using Enterprise.Shared.Outbox;
using Organization.Shared.Workflows;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Organization.Jobs.Services;

public class TemporalOutboxExecutorService(ITemporalClient temporalClient) : ITemporalOutboxExecutor
{
    private static readonly string s_addOrganizationStripePaymentMethodType = typeof(AddOrganizationStripePaymentMethod).ToWorkflowType();

    public async Task StartWorkflowAsync(string workflowType, string? executionArgs, WorkflowOptions workflowOptions)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_addOrganizationStripePaymentMethodType)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<AddOrganizationStripePaymentMethodInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (AddOrganizationStripePaymentMethod workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }
}
