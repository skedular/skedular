using System.Text.Json;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Temporalio.Client;

namespace Booking.Shared.Workflows.Services;

public class TemporalSignalOutboxExecutorService(ITemporalClient temporalClient, ITemporalHelperService temporalHelperService)
    : ITemporalSignalOutboxExecutor
{
    private static readonly string s_payPayBookingByCardSetPaymentStatusAsync =
        typeof(PayBookingByCard).GetMethod(nameof(PayBookingByCard.SetPaymentStatusAsync))!.ToWorkflowSignalType();

    private static readonly string s_payPayBookingByCardDeleteBookingAsync =
        typeof(PayBookingByCard).GetMethod(nameof(PayBookingByCard.DeleteBookingAsync))!.ToWorkflowSignalType();

    public async Task SignalAsync(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (signalType == s_payPayBookingByCardSetPaymentStatusAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingByCard>(workflowId, cancellationToken))
            {
                return;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<SetPaymentStatusArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            await temporalClient
                .GetWorkflowHandle<PayBookingByCard>(workflowId)
                .SignalAsync(workflow => workflow.SetPaymentStatusAsync(input), workflowSignalOptions);
        }
        else if (signalType == s_payPayBookingByCardDeleteBookingAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingByCard>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<PayBookingByCard>(workflowId)
                .SignalAsync(workflow => workflow.DeleteBookingAsync(), workflowSignalOptions);
        }
    }
}
