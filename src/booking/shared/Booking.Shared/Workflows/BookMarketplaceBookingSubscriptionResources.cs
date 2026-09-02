using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record BookMarketplaceBookingSubscriptionResourcesInput(
    string MarketplaceBookingSubscriptionId,
    TimeOnly From,
    TimeOnly Until);

public record BookMarketplaceBookingSubscriptionResourcesState(bool MarketplaceBookingSubscriptionDeleted);

public record MarketplaceBookingSubscriptionDeletedArgs(
    string MarketplaceBookingSubscriptionId,
    TimeOnly From,
    TimeOnly Until);

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
                await ReleaseSubscriptionResourcesAsync(args.MarketplaceBookingSubscriptionId);
                break;
            }

            var response = await Workflow.ExecuteActivityAsync(
                (MarketplaceBookingSubscriptionIntegrations activity) => activity.AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
                    new AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput(
                        args.MarketplaceBookingSubscriptionId,
                        args.From,
                        args.Until)),
                ResourceActivityOptions());
            if (response.Deleted)
            {
                await ReleaseSubscriptionResourcesAsync(args.MarketplaceBookingSubscriptionId);
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

    private static async Task ReleaseSubscriptionResourcesAsync(string subscriptionId)
    {
        try
        {
            await Workflow.ExecuteActivityAsync(
                (MarketplaceBookingSubscriptionIntegrations activity) =>
                    activity.ReleaseMarketplaceBookingSubscriptionResourcesAsync(
                        new ReleaseMarketplaceBookingSubscriptionResourcesInput(subscriptionId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 5,
                        InitialInterval = TimeSpan.FromSeconds(2),
                        MaximumInterval = TimeSpan.FromSeconds(30),
                        BackoffCoefficient = 2,
                    },
                });
        }
        catch
        {
            await Workflow.ExecuteActivityAsync(
                (MarketplaceBookingCleanupIntegrations activity) =>
                    activity.EnqueueAsync(new EnqueueMarketplaceBookingCleanupInput(null, null, subscriptionId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromSeconds(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                });
        }
    }

    private static ActivityOptions ResourceActivityOptions() => new()
    {
        StartToCloseTimeout = TimeSpan.FromMinutes(30),
        TaskQueue = Workflow.Info.TaskQueue,
        RetryPolicy = new RetryPolicy
        {
            MaximumAttempts = 5,
            InitialInterval = TimeSpan.FromSeconds(2),
            MaximumInterval = TimeSpan.FromSeconds(30),
            BackoffCoefficient = 2,
        },
    };
}
