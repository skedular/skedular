using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record BookMarketplaceRecurringResourcesInput(string RecurringBookingId);

public record BookMarketplaceRecurringResourcesState(bool RecurringBookingDeleted);

public record MarketplaceRecurringBookingDeletedArgs(string RecurringBookingId);

[Workflow]
public class BookMarketplaceRecurringResources
{
    private BookMarketplaceRecurringResourcesState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(BookMarketplaceRecurringResourcesInput args)
    {
        _state = new BookMarketplaceRecurringResourcesState(false);

        do
        {
            if (_state.RecurringBookingDeleted)
            {
                await Workflow.ExecuteActivityAsync(
                    (MarketplaceRecurringBookingIntegrations activity) =>
                        activity.ReleaseMarketplaceRecurringBookingResourcesAsync(
                            new ReleaseMarketplaceRecurringBookingResourcesInput(args.RecurringBookingId)),
                    new ActivityOptions
                    {
                        StartToCloseTimeout = TimeSpan.FromMinutes(30),
                        TaskQueue = Workflow.Info.TaskQueue,
                        RetryPolicy = new RetryPolicy { MaximumAttempts = 5, MaximumInterval = TimeSpan.FromSeconds(1) }
                    });

                break;
            }

            var response = await Workflow.ExecuteActivityAsync(
                (MarketplaceRecurringBookingIntegrations activity) =>
                    activity.AdjustRequiredResourcesForMarketplaceRecurringBookingAsync(
                        new AdjustRequiredResourcesForMarketplaceRecurringBookingInput(args.RecurringBookingId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 5, MaximumInterval = TimeSpan.FromSeconds(1) }
                });

            if (response.Deleted)
            {
                await Workflow.ExecuteActivityAsync(
                    (MarketplaceRecurringBookingIntegrations activity) =>
                        activity.ReleaseMarketplaceRecurringBookingResourcesAsync(
                            new ReleaseMarketplaceRecurringBookingResourcesInput(args.RecurringBookingId)),
                    new ActivityOptions
                    {
                        StartToCloseTimeout = TimeSpan.FromMinutes(30),
                        TaskQueue = Workflow.Info.TaskQueue,
                        RetryPolicy = new RetryPolicy { MaximumAttempts = 5, MaximumInterval = TimeSpan.FromSeconds(1) }
                    });
            }

            if (response.Deleted || response.Ended)
            {
                break;
            }

            await Workflow.WaitConditionAsync(() => _state.RecurringBookingDeleted, TimeSpan.FromDays(1));
        } while (true);
    }

    [WorkflowSignal]
    public Task RecurringBookingDeletedAsync(MarketplaceRecurringBookingDeletedArgs args)
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { RecurringBookingDeleted = true };

        return Task.CompletedTask;
    }
}
