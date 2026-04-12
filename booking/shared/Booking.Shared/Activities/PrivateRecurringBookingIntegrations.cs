using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public record AdjustRequiredResourcesForPrivateRecurringBookingInput(string RecurringBookingId);

public record AdjustRequiredResourcesForPrivateRecurringBookingAsyncResponse(bool Deleted, bool Ended);

public record ReleasePrivateRecurringBookingResourcesInput(string RecurringBookingId);

public class PrivateRecurringBookingIntegrations(
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IRecurringBookingScheduleService recurringBookingScheduleService,
    IPrivateBookingService privateBookingService,
    IMapper mapper,
    IRandomHelper randomHelper)
{
    [Activity]
    public async Task<AdjustRequiredResourcesForPrivateRecurringBookingAsyncResponse> AdjustRequiredResourcesForPrivateRecurringBookingAsync(
        AdjustRequiredResourcesForPrivateRecurringBookingInput args)
    {
        // Load recurring booking and stop if it no longer exists.
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted())
        {
            return new AdjustRequiredResourcesForPrivateRecurringBookingAsyncResponse(true, true);
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

        // Refresh all non-customized existing instances that remain in the series.
        // Even if recurrence-generated values did not change, UpdateAsync can now re-adjust resources
        // when previously assigned resources are no longer available.
        var updatedByCustomer = recurringBooking.LastModifiedByCustomer ?? recurringBooking.CreatedByCustomer ?? null;
        var bookingsToRemoveIds = reconciliationPlan.BookingsToRemove.Select(item => item.Id).ToHashSet();

        var existingBookingsToRefresh = existingBookings
            .Where(item => !bookingsToRemoveIds.Contains(item.Id))
            .Where(item => item.HasRecurringInstanceOverrides != true);

        foreach (var existingBooking in existingBookingsToRefresh)
        {
            var expectedBooking = mapper.MapTo(
                recurringBooking,
                mapper.MapTo(existingBooking),
                null,
                DateOnly.FromDateTime(existingBooking.From.UtcDateTime.Date));

            await privateBookingService.UpdateAsync(
                expectedBooking,
                existingBooking,
                updatedByCustomer,
                recurringBooking.InvolvedOrganizations,
                recurringBooking.InvolvedTeams,
                recurringBooking,
                true,
                cancellationToken);
        }

        var bookingsToAdd = reconciliationPlan.MissingBookingDays
            .Select(missingBookingDay => mapper.MapTo(recurringBooking, missingBookingDay))
            .ToList();

        foreach (var booking in bookingsToAdd)
        {
            booking.Id = randomHelper.Generate();

            await privateBookingService.AddAsync(
                booking,
                recurringBooking.InvolvedCustomers.First(),
                recurringBooking.InvolvedOrganizations,
                recurringBooking.InvolvedTeams,
                recurringBooking,
                cancellationToken);
        }

        // If recurrence has no future valid booking days, the workflow can terminate.
        return new AdjustRequiredResourcesForPrivateRecurringBookingAsyncResponse(false, !reconciliationPlan.HasMoreRequiredBookingDays);
    }

    [Activity]
    public async Task ReleasePrivateRecurringBookingResourcesAsync(ReleasePrivateRecurringBookingResourcesInput args)
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
