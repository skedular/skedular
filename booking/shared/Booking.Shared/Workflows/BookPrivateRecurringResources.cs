using Booking.Shared.Activities;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record BookPrivateRecurringResourcesInput(string RecurringBookingId);

public record BookPrivateRecurringResourcesState(ICollection<bool> UpdateQueue, bool RecurringBookingDeleted);

public record RecurringBookingUpdatedArgs(string RecurringBookingId);

public record RecurringBookingDeletedArgs(string RecurringBookingId);

[Workflow]
public class BookPrivateRecurringResources
{
    private BookPrivateRecurringResourcesState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(BookPrivateRecurringResourcesInput args)
    {
        _state = new BookPrivateRecurringResourcesState([true], false);

        do
        {
            if (_state.RecurringBookingDeleted)
            {
                await Workflow.ExecuteActivityAsync(
                    (PrivateRecurringBookingIntegrations activity) =>
                        activity.ReleaseRecurringBookingResourcesAsync(new ReleaseRecurringBookingResourcesInput(args.RecurringBookingId)),
                    new ActivityOptions
                    {
                        StartToCloseTimeout = TimeSpan.FromMinutes(30),
                        TaskQueue = Workflow.Info.TaskQueue,
                        RetryPolicy = new RetryPolicy { MaximumAttempts = 5, MaximumInterval = TimeSpan.FromSeconds(1) }
                    });

                break;
            }

            var response = await Workflow.ExecuteActivityAsync(
                (PrivateRecurringBookingIntegrations activity) =>
                    activity.AdjustRequiredResourcesForRecurringBookingAsync(
                        new AdjustRequiredResourcesForRecurringBookingInput(args.RecurringBookingId)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(30),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy { MaximumAttempts = 5, MaximumInterval = TimeSpan.FromSeconds(1) }
                });

            if (response.Deleted)
            {
                await Workflow.ExecuteActivityAsync(
                    (PrivateRecurringBookingIntegrations activity) =>
                        activity.ReleaseRecurringBookingResourcesAsync(new ReleaseRecurringBookingResourcesInput(args.RecurringBookingId)),
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

            _state = _state with { UpdateQueue = _state.UpdateQueue.Skip(1).ToList() };

            await Workflow.WaitConditionAsync(() => _state.UpdateQueue.Any() || _state.RecurringBookingDeleted, TimeSpan.FromDays(1));
        } while (true);
    }

    [WorkflowSignal]
    public Task RecurringBookingUpdatedAsync(RecurringBookingUpdatedArgs args)
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { UpdateQueue = _state.UpdateQueue.Concat([true]).ToList() };

        return Task.CompletedTask;
    }

    [WorkflowSignal]
    public Task RecurringBookingDeletedAsync(RecurringBookingDeletedArgs args)
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { RecurringBookingDeleted = true };

        return Task.CompletedTask;
    }
}
