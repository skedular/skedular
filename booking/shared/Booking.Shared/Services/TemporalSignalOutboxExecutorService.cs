using System.Text.Json;
using Booking.Shared.Workflows.Payment;
using Booking.Shared.Workflows.Payment.PayViaBankTransfer;
using Booking.Shared.Workflows.Payment.PayViaCard;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Temporalio.Client;

namespace Booking.Shared.Services;

public class TemporalSignalOutboxExecutorService(ITemporalClient temporalClient, ITemporalHelperService temporalHelperService)
    : ITemporalSignalOutboxExecutor
{
    private static readonly string s_payBookingViaCardSetPaymentStatusAsync =
        typeof(PayBookingViaCard).GetMethod(nameof(PayBookingViaCard.SetPaymentStatusAsync))!.ToWorkflowSignalType();

    private static readonly string s_payBookingViaCardDeleteBookingAsync =
        typeof(PayBookingViaCard).GetMethod(nameof(PayBookingViaCard.DeleteBookingAsync))!.ToWorkflowSignalType();

    private static readonly string s_payBookingViaBankTransferSetPaymentStatusAsync =
        typeof(PayBookingViaBankTransfer).GetMethod(nameof(PayBookingViaBankTransfer.SetPaymentStatusAsync))!.ToWorkflowSignalType();

    private static readonly string s_payBookingViaBankTransferDeleteBookingAsync =
        typeof(PayBookingViaBankTransfer).GetMethod(nameof(PayBookingViaBankTransfer.DeleteBookingAsync))!.ToWorkflowSignalType();

    public async Task SignalAsync(
        string workflowId,
        string signalType,
        string? executionArgs,
        WorkflowSignalOptions workflowSignalOptions,
        CancellationToken cancellationToken)
    {
        await temporalClient.Connection.ConnectAsync();

        if (signalType == s_payBookingViaCardSetPaymentStatusAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingViaCard>(workflowId, cancellationToken))
            {
                return;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<SetPaymentStatusArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            await temporalClient
                .GetWorkflowHandle<PayBookingViaCard>(workflowId)
                .SignalAsync(workflow => workflow.SetPaymentStatusAsync(input), workflowSignalOptions);
        }
        else if (signalType == s_payBookingViaCardDeleteBookingAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingViaCard>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<PayBookingViaCard>(workflowId)
                .SignalAsync(workflow => workflow.DeleteBookingAsync(), workflowSignalOptions);
        }
        else if (signalType == s_payBookingViaBankTransferSetPaymentStatusAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingViaBankTransfer>(workflowId, cancellationToken))
            {
                return;
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
            var input = JsonSerializer.Deserialize<SetPaymentStatusArgs>(executionArgs);
            ArgumentNullException.ThrowIfNull(input);

            await temporalClient
                .GetWorkflowHandle<PayBookingViaBankTransfer>(workflowId)
                .SignalAsync(workflow => workflow.SetPaymentStatusAsync(input), workflowSignalOptions);
        }
        else if (signalType == s_payBookingViaBankTransferDeleteBookingAsync)
        {
            if (!await temporalHelperService.IsRunningAsync<PayBookingViaBankTransfer>(workflowId, cancellationToken))
            {
                return;
            }

            await temporalClient
                .GetWorkflowHandle<PayBookingViaBankTransfer>(workflowId)
                .SignalAsync(workflow => workflow.DeleteBookingAsync(), workflowSignalOptions);
        }
    }
}
