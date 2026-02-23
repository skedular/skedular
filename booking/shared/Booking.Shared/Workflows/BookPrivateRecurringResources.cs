using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record BookPrivateRecurringResourcesInput(string RecurringBookingId);

public record BookPrivateRecurringResourcesState(ICollection<bool> UpdateQueue, bool RecurringBookingDeleted);

[Workflow]
public class BookPrivateRecurringResources
{
    private BookPrivateRecurringResourcesState? _bookPrivateRecurringResourcesState;

    [WorkflowRun]
    public async Task ExecuteAsync(BookPrivateRecurringResourcesInput args) =>
        _bookPrivateRecurringResourcesState = new BookPrivateRecurringResourcesState([true], false);

    [WorkflowSignal]
    public Task RecurringBookingUpdatedAsync()
    {
        ArgumentNullException.ThrowIfNull(_bookPrivateRecurringResourcesState);

        _bookPrivateRecurringResourcesState = _bookPrivateRecurringResourcesState with
        {
            UpdateQueue = _bookPrivateRecurringResourcesState.UpdateQueue.Concat([true]).ToList()
        };

        return Task.CompletedTask;
    }

    [WorkflowSignal]
    public Task RecurringBookingDeletedAsync()
    {
        ArgumentNullException.ThrowIfNull(_bookPrivateRecurringResourcesState);

        _bookPrivateRecurringResourcesState = _bookPrivateRecurringResourcesState with { RecurringBookingDeleted = true };

        return Task.CompletedTask;
    }
}
