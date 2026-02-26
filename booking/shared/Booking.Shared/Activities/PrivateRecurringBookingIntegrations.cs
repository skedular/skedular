using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public record AdjustRequiredResourcesForRecurringBookingInput(string RecurringBookingId);

public record AdjustRequiredResourcesForRecurringBookingAsyncResponse(bool Deleted, bool Ended);

public record ReleaseRecurringBookingResourcesInput(string RecurringBookingId);

public class PrivateRecurringBookingIntegrations(
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IRecurringBookingScheduleService recurringBookingScheduleService,
    IPrivateBookingService privateBookingService,
    IMapper mapper,
    IRandomHelper randomHelper)
{
    [Activity]
    public async Task<AdjustRequiredResourcesForRecurringBookingAsyncResponse> AdjustRequiredResourcesForRecurringBookingAsync(
        AdjustRequiredResourcesForRecurringBookingInput args)
    {
        // Load recurring booking and stop if it no longer exists.
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted())
        {
            return new AdjustRequiredResourcesForRecurringBookingAsyncResponse(true, false);
        }

        // We reconcile from "today" onward.
        var now = timeProvider.GetUtcNow();
        var from = new DateTimeOffset(now.UtcDateTime.Date, TimeSpan.Zero);
        var until = from.AddMonths(1);

        // Pull all future bookings for this recurrence (no upper bound).
        var existingBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
            recurringBooking.Id,
            from,
            null,
            cancellationToken);

        // Build a reusable recurrence plan with missing/obsolete booking decisions.
        var reconciliationPlan = recurringBookingScheduleService.GetReconciliationPlan(
            recurringBooking,
            from,
            until,
            existingBookings);

        foreach (var existingBooking in reconciliationPlan.BookingsToRemove)
        {
            await privateBookingService.DeleteAsync(existingBooking, null, cancellationToken);
        }

        foreach (var booking in reconciliationPlan.MissingBookingDays.Select(missingBookingDay => mapper.MapTo(recurringBooking, missingBookingDay)))
        {
            booking.Id = randomHelper.Generate();

            await privateBookingService.AddAsync(
                booking,
                recurringBooking.InvolvedCustomers.First(),
                recurringBooking.InvolvedOrganizations,
                recurringBooking.InvolvedTeams,
                cancellationToken);
        }

        // If recurrence has no future valid booking days, the workflow can terminate.
        return new AdjustRequiredResourcesForRecurringBookingAsyncResponse(false, !reconciliationPlan.HasMoreRequiredBookingDays);
    }

    [Activity]
    public async Task ReleaseRecurringBookingResourcesAsync(ReleaseRecurringBookingResourcesInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var from = new DateTimeOffset(now.UtcDateTime.Date, TimeSpan.Zero);

        // Pull all future bookings for this recurrence (no upper bound).
        var existingBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
            recurringBooking.Id,
            from,
            null,
            cancellationToken);

        foreach (var existingBooking in existingBookings)
        {
            await privateBookingService.DeleteAsync(existingBooking, recurringBooking.DeletedByCustomer, cancellationToken);
        }
    }
}
