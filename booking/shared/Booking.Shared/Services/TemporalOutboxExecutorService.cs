using System.Text.Json;
using Booking.Shared.Workflows.Payment.PayViaBankTransfer;
using Booking.Shared.Workflows.Payment.PayViaCard;
using Enterprise.Shared.Outbox;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Booking.Shared.Services;

public class TemporalOutboxExecutorService(ITemporalClient temporalClient) : ITemporalOutboxExecutor
{
    private static readonly string s_payBookingViaCard = typeof(PayBookingViaCard).ToWorkflowType();
    private static readonly string s_payBookingViaBankTransfer = typeof(PayBookingViaBankTransfer).ToWorkflowType();

    public async Task StartWorkflowAsync(
        string workflowType,
        string? executionArgs,
        WorkflowOptions workflowOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_payBookingViaCard)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<PayBookingViaCardInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((PayBookingViaCard workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
        else if (workflowType == s_payBookingViaBankTransfer)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<PayBookingViaBankTransferInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync((PayBookingViaBankTransfer workflow) => workflow.ExecuteAsync(input), workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }
}
