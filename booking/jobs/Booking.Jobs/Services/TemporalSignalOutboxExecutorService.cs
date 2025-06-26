using System.Text.Json;
using Booking.Shared.Workflows;
using Enterprise.Shared.Outbox;
using Temporalio.Client;

namespace Booking.Jobs.Services;

public class TemporalSignalOutboxExecutorService(ITemporalClient temporalClient) : ITemporalSignalOutboxExecutor
{
    private static readonly string s_payBookingUsingStripeCheckoutSessionSetPaymentStatusAsync =
        typeof(PayBookingUsingStripeCheckoutSession).GetMethod(nameof(PayBookingUsingStripeCheckoutSession.SetPaymentStatusAsync))!
            .ToWorkflowSignalType();

    private static readonly string s_payBookingUsingStripeCheckoutSessionDeleteBookingAsync =
        typeof(PayBookingUsingStripeCheckoutSession).GetMethod(nameof(PayBookingUsingStripeCheckoutSession.DeleteBookingAsync))!
            .ToWorkflowSignalType();

    public async Task SignalAsync(string workflowId, string signalType, string? executionArgs, WorkflowSignalOptions workflowSignalOptions)
    {
        await temporalClient.Connection.ConnectAsync();

        if (signalType == s_payBookingUsingStripeCheckoutSessionSetPaymentStatusAsync)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<SetPaymentStatusArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            var handle = temporalClient.GetWorkflowHandle<PayBookingUsingStripeCheckoutSession>(workflowId);
            ArgumentNullException.ThrowIfNull(handle);

            await handle.SignalAsync(workflow => workflow.SetPaymentStatusAsync(input), workflowSignalOptions);
        }
        else if (signalType == s_payBookingUsingStripeCheckoutSessionDeleteBookingAsync)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<DeleteBookingArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            var handle = temporalClient.GetWorkflowHandle<PayBookingUsingStripeCheckoutSession>(workflowId);
            ArgumentNullException.ThrowIfNull(handle);

            await handle.SignalAsync(workflow => workflow.DeleteBookingAsync(input), workflowSignalOptions);
        }
    }
}
