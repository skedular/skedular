using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record PayRecurringBookingViaBankTransferInput(string RecurringBookingId, DateTimeOffset ExpiryDate, ICollection<string> InvoiceEmailList);

public record PayRecurringBookingViaBankTransferState(string? PaymentStatus);

[Workflow]
public class PayRecurringBookingViaBankTransfer
{
    private PayRecurringBookingViaBankTransferState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(PayRecurringBookingViaBankTransferInput args)
    {
        _state = new PayRecurringBookingViaBankTransferState(null);

        try
        {
            await Workflow.ExecuteActivityAsync(
                (BookingIntegrations activity) => activity.CalculateRecurringBookingDifferentAmountsAsync(
                    new CalculateRecurringBookingDifferentAmountsInput(args.RecurringBookingId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });

            await Workflow.ExecuteActivityAsync(
                (InvoiceIntegrations activity) =>
                    activity.GenerateAndSendRecurringInvoiceAsync(
                        new GenerateAndSendRecurringInvoiceInput(args.RecurringBookingId, false, args.InvoiceEmailList)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(2),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });

            if (!await Workflow.WaitConditionAsync(() => _state.PaymentStatus is not null, GetDelayDuration(args)))
            {
                await Workflow.ExecuteActivityAsync(
                    (MarketplaceBookingSubscriptionIntegrations activity) => activity.ReleaseRecurringBookingResourcesAsync(
                        new ReleaseRecurringBookingResourcesInput(args.RecurringBookingId)),
                    new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });

                return;
            }

            if (string.IsNullOrWhiteSpace(_state.PaymentStatus) ||
                _state.PaymentStatus.ToPaymentStatus() is not (PaymentStatus.Confirmed or PaymentStatus.NoPaymentRequired))
            {
                await Workflow.ExecuteActivityAsync(
                    (MarketplaceBookingSubscriptionIntegrations activity) => activity.ReleaseRecurringBookingResourcesAsync(
                        new ReleaseRecurringBookingResourcesInput(args.RecurringBookingId)),
                    new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });

                return;
            }
        }
        catch (Exception)
        {
            await Workflow.ExecuteActivityAsync(
                (MarketplaceBookingSubscriptionIntegrations activity) => activity.ReleaseRecurringBookingResourcesAsync(
                    new ReleaseRecurringBookingResourcesInput(args.RecurringBookingId)),
                new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });

            return;
        }

        await Workflow.ExecuteActivityAsync(
            (InvoiceIntegrations activity) =>
                activity.GenerateAndSendRecurringInvoiceAsync(
                    new GenerateAndSendRecurringInvoiceInput(args.RecurringBookingId, true, args.InvoiceEmailList)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
            });
    }

    [WorkflowSignal]
    public Task SetPaymentStatusAsync(SetPaymentStatusArgs args)
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { PaymentStatus = args.PaymentStatus };

        return Task.CompletedTask;
    }

    private static TimeSpan GetDelayDuration(PayRecurringBookingViaBankTransferInput args)
    {
        var delayDuration = args.ExpiryDate - Workflow.UtcNow;
        if (delayDuration <= TimeSpan.Zero)
        {
            throw new ApplicationFailureException($"Failed to complete recurring booking {args.RecurringBookingId} paid via bank transfer");
        }

        return delayDuration;
    }
}
