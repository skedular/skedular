using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows.Payment.PayViaCard;

[Workflow]
public class PayBookingViaCard
{
    private PayBookingViaCardState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(PayBookingViaCardInput args)
    {
        _state = new PayBookingViaCardState(null, false);

        try
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

            var upsertProductAndPricingResponse = await Workflow.ExecuteActivityAsync(
                (StripeIntegrations activity) => activity.UpsertProductAndPricingAsync(
                    new UpsertProductAndPricingInput(args.BookingId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = GetDelayDuration(args),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });
            if (upsertProductAndPricingResponse is null)
            {
                return;
            }

            var upsertBookingRelatedStripeCustomerResponse = await Workflow.ExecuteActivityAsync(
                (StripeIntegrations activity) => activity.UpsertBookingRelatedStripeCustomerAsync(
                    new UpsertBookingRelatedStripeCustomerInput(args.BookingId, upsertProductAndPricingResponse.StripeConnectAccountId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = GetDelayDuration(args),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });
            if (upsertBookingRelatedStripeCustomerResponse is null)
            {
                return;
            }

            var createCheckoutSessionAsyncResponse = await Workflow.ExecuteActivityAsync(
                (StripeIntegrations activity) => activity.CreateCheckoutSessionAsync(
                    new CreateCheckoutSessionAsyncInput(
                        args.BookingId,
                        upsertProductAndPricingResponse.StripeConnectAccountId,
                        upsertBookingRelatedStripeCustomerResponse.StripeCustomerId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = GetDelayDuration(args),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });
            if (createCheckoutSessionAsyncResponse is null ||
                createCheckoutSessionAsyncResponse.PaymentStatus.ToPaymentStatus() is PaymentStatus.Confirmed
                    or PaymentStatus.NoPaymentRequired)
            {
                return;
            }

            if (!await Workflow.WaitConditionAsync(() => _state.PaymentStatus is not null || _state.BookingDeleted, GetDelayDuration(args)))
            {
                await Workflow.ExecuteActivityAsync(
                    (BookingIntegrations activity) => activity.ReleaseBookingResourcesAsync(
                        new ReleaseBookingResourcesInput(args.BookingId)),
                    new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });

                return;
            }

            if (!_state.BookingDeleted && (string.IsNullOrWhiteSpace(_state.PaymentStatus) ||
                                           _state.PaymentStatus.ToPaymentStatus() is not (PaymentStatus.Confirmed
                                               or PaymentStatus.NoPaymentRequired)))
            {
                await Workflow.ExecuteActivityAsync(
                    (BookingIntegrations activity) => activity.ReleaseBookingResourcesAsync(
                        new ReleaseBookingResourcesInput(args.BookingId)),
                    new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });

                return;
            }
        }
        catch (Exception)
        {
            await Workflow.ExecuteActivityAsync(
                (BookingIntegrations activity) => activity.ReleaseBookingResourcesAsync(
                    new ReleaseBookingResourcesInput(args.BookingId)),
                new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });
        }

        await Workflow.ExecuteActivityAsync(
            (InvoiceIntegrations activity) =>
                activity.GenerateAndSendInvoiceAsync(new GenerateAndSendInvoiceInput(args.BookingId, true, args.InvoiceEmailList)),
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

    [WorkflowSignal]
    public Task DeleteBookingAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { BookingDeleted = true };

        return Task.CompletedTask;
    }

    private static TimeSpan GetDelayDuration(PayBookingViaCardInput args)
    {
        var delayDuration = args.ExpiryDate - TimeProvider.System.GetUtcNow();
        if (delayDuration <= TimeSpan.Zero)
        {
            throw new ApplicationFailureException($"Failed to complete booking {args.BookingId} paid via card");
        }

        return delayDuration;
    }
}
