using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public record AdjustRequiredResourcesForRecurringBookingInput(string RecurringBookingId);

public record AdjustRequiredResourcesForRecurringBookingAsyncResponse(bool Deleted, bool Ended);

public record ReleaseRecurringBookingResourcesInput(string RecurringBookingId);

public class PrivateRecurringBookingIntegrations(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IRecurringBookingScheduleService recurringBookingScheduleService,
    IPrivateBookingService privateBookingService)
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

        // Compute the required days for the near horizon and detect missing booking days.
        var requiredBookingDaysResponse = recurringBookingScheduleService.GetRequiredBookingDays(recurringBooking, from, until);
        var requiredBookingDays = requiredBookingDaysResponse.Days.ToHashSet();
        var existingBookingDays = existingBookings.Select(booking => DateOnly.FromDateTime(booking.From.UtcDateTime.Date)).ToHashSet();
        var missingBookingDays = requiredBookingDays.Where(day => !existingBookingDays.Contains(day)).ToList();

        // Collect existing bookings that should no longer exist.
        var bookingsToRemove = new List<Database.Entities.Booking>();

        if (existingBookings.Count > 0)
        {
            // Expand expected days up to the furthest booked day so we can validate all future bookings returned above.
            var maxBookingDay = existingBookings.Select(booking => DateOnly.FromDateTime(booking.From.UtcDateTime.Date)).Max();
            var evaluationUntil = new DateTimeOffset(maxBookingDay.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc), TimeSpan.Zero);
            var expectedBookingDays =
                recurringBookingScheduleService.GetRequiredBookingDays(recurringBooking, from, evaluationUntil).Days.ToHashSet();
            var groupedByDay = existingBookings.GroupBy(booking => DateOnly.FromDateTime(booking.From.UtcDateTime.Date));

            foreach (var dayGroup in groupedByDay)
            {
                // Remove all bookings on days that are no longer part of the recurrence.
                if (!expectedBookingDays.Contains(dayGroup.Key))
                {
                    bookingsToRemove.AddRange(dayGroup);

                    continue;
                }

                // Keep one booking per expected day and mark extras for removal.
                bookingsToRemove.AddRange(dayGroup.OrderBy(booking => booking.From).Skip(1));
            }
        }

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var existingBooking in bookingsToRemove)
        {
            await privateBookingService.DeleteAsync(false, existingBooking, null, cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);

        // If recurrence has no future valid booking days, the workflow can terminate.
        return new AdjustRequiredResourcesForRecurringBookingAsyncResponse(false, !requiredBookingDaysResponse.HasMoreRequiredBookingDays);
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

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var existingBooking in existingBookings)
        {
            await privateBookingService.DeleteAsync(false, existingBooking, recurringBooking.DeletedByCustomer, cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }
}
