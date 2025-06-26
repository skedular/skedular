using System.Text.Json;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Temporalio.Client;

namespace Booking.Shared.Workflows.Services;

public class TemporalSignalOutboxExecutorService(ITemporalClient temporalClient, ITemporalHelperService temporalHelperService)
    : ITemporalSignalOutboxExecutor
{
    private static readonly string s_payBookingUsingStripeCheckoutSessionSetPaymentStatusAsync =
        typeof(PayBookingUsingStripeCheckoutSession).GetMethod(nameof(PayBookingUsingStripeCheckoutSession.SetPaymentStatusAsync))!
            .ToWorkflowSignalType();

    private static readonly string s_payBookingUsingStripeCheckoutSessionDeleteBookingAsync =
        typeof(PayBookingUsingStripeCheckoutSession).GetMethod(nameof(PayBookingUsingStripeCheckoutSession.DeleteBookingAsync))!
            .ToWorkflowSignalType();

    public async Task SignalAsync(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (signalType == s_payBookingUsingStripeCheckoutSessionSetPaymentStatusAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingUsingStripeCheckoutSession>(workflowId, cancellationToken))
            {
                return;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<SetPaymentStatusArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            await temporalClient
                .GetWorkflowHandle<PayBookingUsingStripeCheckoutSession>(workflowId)
                .SignalAsync(workflow => workflow.SetPaymentStatusAsync(input), workflowSignalOptions);
        }
        else if (signalType == s_payBookingUsingStripeCheckoutSessionDeleteBookingAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingUsingStripeCheckoutSession>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<PayBookingUsingStripeCheckoutSession>(workflowId)
                .SignalAsync(workflow => workflow.DeleteBookingAsync(), workflowSignalOptions);
        }
    }
}
