using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record BookMarketplaceBookingSubscriptionResourcesInput(string MarketplaceBookingSubscriptionId);

public record BookMarketplaceBookingSubscriptionResourcesState(bool MarketplaceBookingSubscriptionDeleted);

public record MarketplaceBookingSubscriptionDeletedArgs(string MarketplaceBookingSubscriptionId);

[Workflow]
public class BookMarketplaceBookingSubscriptionResources
{
    private BookMarketplaceBookingSubscriptionResourcesState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(BookMarketplaceBookingSubscriptionResourcesInput args)
    {
        _state = new BookMarketplaceBookingSubscriptionResourcesState(false);

        do
        {
            if (_state.MarketplaceBookingSubscriptionDeleted)
            {
                await Workflow.ExecuteActivityAsync(
                    (MarketplaceBookingSubscriptionIntegrations activity) =>
                        activity.ReleaseMarketplaceBookingSubscriptionResourcesAsync(
                            new ReleaseMarketplaceBookingSubscriptionResourcesInput(args.MarketplaceBookingSubscriptionId)),
                    new ActivityOptions
                    {
                        StartToCloseTimeout = TimeSpan.FromMinutes(30),
                        TaskQueue = Workflow.Info.TaskQueue,
                        RetryPolicy = new RetryPolicy
                        {
                            MaximumAttempts = 5,
                            MaximumInterval = TimeSpan.FromSeconds(1),
                        },
                    });

                break;
            }

            var response = await Workflow.ExecuteActivityAsync(
                (MarketplaceBookingSubscriptionIntegrations activity) =>
                    activity.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                        new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput(args.MarketplaceBookingSubscriptionId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 5,
                        MaximumInterval = TimeSpan.FromSeconds(1),
                    },
                });

            if (response.Deleted)
            {
                await Workflow.ExecuteActivityAsync(
                    (MarketplaceBookingSubscriptionIntegrations activity) =>
                        activity.ReleaseMarketplaceBookingSubscriptionResourcesAsync(
                            new ReleaseMarketplaceBookingSubscriptionResourcesInput(args.MarketplaceBookingSubscriptionId)),
                    new ActivityOptions
                    {
                        StartToCloseTimeout = TimeSpan.FromMinutes(30),
                        TaskQueue = Workflow.Info.TaskQueue,
                        RetryPolicy = new RetryPolicy
                        {
                            MaximumAttempts = 5,
                            MaximumInterval = TimeSpan.FromSeconds(1),
                        },
                    });
            }

            if (response.Deleted || response.Ended)
            {
                break;
            }

            await Workflow.WaitConditionAsync(() => _state.MarketplaceBookingSubscriptionDeleted, TimeSpan.FromDays(1));
        } while (true);
    }

    [WorkflowSignal]
    public Task MarketplaceBookingSubscriptionDeletedAsync(MarketplaceBookingSubscriptionDeletedArgs args)
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with
        {
            MarketplaceBookingSubscriptionDeleted = true,
        };

        return Task.CompletedTask;
    }
}
