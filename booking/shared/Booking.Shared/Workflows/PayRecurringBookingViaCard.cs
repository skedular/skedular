using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record PayRecurringBookingViaCardInput(string RecurringBookingId, DateTimeOffset ExpiryDate, ICollection<string> InvoiceEmailList);

public record PayRecurringBookingViaCardState(string? PaymentStatus, bool RecurringBookingDeleted);

[Workflow]
public class PayRecurringBookingViaCard
{
    private PayRecurringBookingViaCardState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(PayRecurringBookingViaCardInput args)
    {
        _state = new PayRecurringBookingViaCardState(null, false);

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
                        new GenerateAndSendRecurringInvoiceInput(args.RecurringBookingId, args.InvoiceEmailList)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(2),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });

            var upsertProductAndPricingResponse = await Workflow.ExecuteActivityAsync(
                (StripeIntegrations activity) => activity.UpsertRecurringBookingProductAndPricingAsync(
                    new UpsertRecurringBookingProductAndPricingInput(args.RecurringBookingId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });
            if (upsertProductAndPricingResponse is null)
            {
                return;
            }

            var upsertBookingRelatedStripeCustomerResponse = await Workflow.ExecuteActivityAsync(
                (StripeIntegrations activity) => activity.UpsertRecurringBookingRelatedStripeCustomerAsync(
                    new UpsertRecurringBookingRelatedStripeCustomerInput(args.RecurringBookingId,
                        upsertProductAndPricingResponse.StripeConnectAccountId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });
            if (upsertBookingRelatedStripeCustomerResponse is null)
            {
                return;
            }

            var createCheckoutSessionAsyncResponse = await Workflow.ExecuteActivityAsync(
                (StripeIntegrations activity) => activity.CreateRecurringBookingCheckoutSessionAsync(
                    new CreateRecurringBookingCheckoutSessionAsyncInput(
                        args.RecurringBookingId,
                        upsertProductAndPricingResponse.StripeConnectAccountId,
                        upsertBookingRelatedStripeCustomerResponse.StripeCustomerId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 3, MaximumInterval = TimeSpan.FromSeconds(5) }
                });
            if (createCheckoutSessionAsyncResponse is null ||
                createCheckoutSessionAsyncResponse.PaymentStatus.ToPaymentStatus() is PaymentStatus.Confirmed
                    or PaymentStatus.NoPaymentRequired)
            {
                return;
            }

            if (!await Workflow.WaitConditionAsync(() => _state.PaymentStatus is not null || _state.RecurringBookingDeleted, GetDelayDuration(args)))
            {
                await Workflow.ExecuteActivityAsync(
                    (MarketplaceBookingSubscriptionIntegrations activity) => activity.ReleaseRecurringBookingResourcesAsync(
                        new ReleaseRecurringBookingResourcesInput(args.RecurringBookingId)),
                    new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });

                return;
            }

            if (_state.RecurringBookingDeleted)
            {
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

        if (_state.RecurringBookingDeleted)
        {
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
    public Task DeleteRecurringBookingAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { RecurringBookingDeleted = true };

        return Task.CompletedTask;
    }

    private static TimeSpan GetDelayDuration(PayRecurringBookingViaCardInput args)
    {
        var delayDuration = args.ExpiryDate - Workflow.UtcNow;
        if (delayDuration <= TimeSpan.Zero)
        {
            throw new ApplicationFailureException($"Failed to complete recurring booking {args.RecurringBookingId} paid via card");
        }

        return delayDuration;
    }
}
