using System.Text.Json;
using Enterprise.Shared.Outbox;
using Customer.Shared.Workflows;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Customer.Jobs.Services;

public class TemporalOutboxExecutorService(ITemporalClient temporalClient) : ITemporalOutboxExecutor
{
    private static readonly string s_addCustomerStripePaymentMethodType = typeof(AddCustomerStripePaymentMethod).ToWorkflowType();

    public async Task StartWorkflowAsync(string workflowType, string? executionArgs, WorkflowOptions workflowOptions)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_addCustomerStripePaymentMethodType)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<AddCustomerStripePaymentMethodInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (AddCustomerStripePaymentMethod workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }
}
