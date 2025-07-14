using Api.Shared.Services.Models;
using Booking.Shared.Workflows.Activities;
using Temporalio.Common;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

[Workflow]
public class PayBookingViaBankTransfer
{
    private PayBookingViaBankTransferState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(PayBookingViaBankTransferInput args)
    {
        _state = new PayBookingViaBankTransferState(null, false);

        try
        {
            if (args.SendInvoice)
            {
                await Workflow.ExecuteActivityAsync(
                    (InvoiceIntegrations activity) =>
                        activity.GenerateAndSendInvoiceAsync(new GenerateAndSendInvoiceInput(args.BookingId, false, args.InvoiceEmailList)),
                    new ActivityOptions
                    {
                        StartToCloseTimeout = TimeSpan.FromMinutes(2),
                        TaskQueue = Workflow.Info.TaskQueue,
                        RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                    });
            }

            if (!await Workflow.WaitConditionAsync(() => _state.PaymentStatus is not null || _state.BookingDeleted, GetDelayDuration(args)))
            {
                await Workflow.ExecuteActivityAsync(
                    (BookingIntegrations activity) => activity.ReleaseBookingResourcesAsync(
                        new ReleaseBookingResourcesInput(args.BookingId)),
                    new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });

                return;
            }

            if (_state.BookingDeleted || (!string.IsNullOrWhiteSpace(_state.PaymentStatus) &&
                                          _state.PaymentStatus.ToPaymentStatus() is PaymentStatus.Confirmed
                                              or PaymentStatus.NoPaymentRequired))
            {
                return;
            }

            await Workflow.ExecuteActivityAsync(
                (BookingIntegrations activity) => activity.ReleaseBookingResourcesAsync(
                    new ReleaseBookingResourcesInput(args.BookingId)),
                new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });
        }
        catch (Exception)
        {
            await Workflow.ExecuteActivityAsync(
                (BookingIntegrations activity) => activity.ReleaseBookingResourcesAsync(
                    new ReleaseBookingResourcesInput(args.BookingId)),
                new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });
        }
    }

    [WorkflowSignal]
    public Task SetPaymentStatusAsync(SetPaymentStatusArgs args)
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { PaymentStatus = args.PaymentStatus };

        return Task.CompletedTask;
    }

    [WorkflowSignal]
    public Task DeleteBookingAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { BookingDeleted = true };

        return Task.CompletedTask;
    }

    private static TimeSpan GetDelayDuration(PayBookingViaBankTransferInput args)
    {
        var delayDuration = args.ExpiryDate - TimeProvider.System.GetUtcNow();
        if (delayDuration <= TimeSpan.Zero)
        {
            throw new ApplicationFailureException($"Failed to complete booking {args.BookingId} paid via bank transfer");
        }

        return delayDuration;
    }
}
