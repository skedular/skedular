using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public record AdjustRequiredResourcesForMarketplaceRecurringBookingInput(string RecurringBookingId);

public record AdjustRequiredResourcesForMarketplaceRecurringBookingAsyncResponse(bool Deleted, bool Ended);

public record ReleaseMarketplaceRecurringBookingResourcesInput(string RecurringBookingId);

public class MarketplaceRecurringBookingIntegrations(
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IRecurringBookingScheduleService recurringBookingScheduleService,
    IMarketplaceBookingService marketplaceBookingService,
    IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
    IMapper mapper,
    IRandomHelper randomHelper)
{
    [Activity]
    public async Task<AdjustRequiredResourcesForMarketplaceRecurringBookingAsyncResponse> AdjustRequiredResourcesForMarketplaceRecurringBookingAsync(
        AdjustRequiredResourcesForMarketplaceRecurringBookingInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted())
        {
            return new AdjustRequiredResourcesForMarketplaceRecurringBookingAsyncResponse(true, true);
        }

        var marketplaceBookingEntity = recurringBooking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBookingEntity);

        var now = timeProvider.GetUtcNow();
        var from = new DateTimeOffset(now.UtcDateTime.Date, TimeSpan.Zero);
        var until = from.AddMonths(1);

        var existingBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
            recurringBooking.Id,
            from,
            null,
            cancellationToken);

        var reconciliationPlan = recurringBookingScheduleService.GetReconciliationPlan(
            recurringBooking,
            from,
            until,
            existingBookings);

        // Clear stale generated instances before repairing the remaining booking days.
        foreach (var existingBooking in reconciliationPlan.BookingsToRemove)
        {
            await marketplaceBookingService.DeleteAsync(existingBooking, null, cancellationToken);
        }

        var updatedByCustomer = recurringBooking.LastModifiedByCustomer ?? recurringBooking.CreatedByCustomer;
        ArgumentNullException.ThrowIfNull(updatedByCustomer);

        var bookingsToRemoveIds = reconciliationPlan.BookingsToRemove.Select(item => item.Id).ToHashSet();
        var existingBookingsToRefresh = existingBookings
            .Where(item => !bookingsToRemoveIds.Contains(item.Id))
            .Where(item => item.HasRecurringInstanceOverrides != true)
            .ToList();
        var useOpeningHoursWindow =
            marketplaceBookingOpeningHoursService.ShouldUseLocationOpeningHoursWindow(marketplaceBookingEntity.ProductPricing.PurchaseCadence);

        foreach (var existingBooking in existingBookingsToRefresh)
        {
            // Existing marketplace instances keep the original time window they were created with.
            // Reconciliation here only repairs resource assignment; it does not move the booking
            // to match later opening-hours changes.
            await marketplaceBookingService.AdjustRequiredResourcesAsync(existingBooking, cancellationToken);
        }

        var customer = recurringBooking.InvolvedCustomers.Count == 1
            ? await repositoryFactory.CustomerRepository.GetByIdAsync(recurringBooking.InvolvedCustomers.First().Id, true, cancellationToken)
            : null;
        var preferredLocationId = existingBookingsToRefresh
            .Select(marketplaceBookingOpeningHoursService.ResolveLocation)
            .FirstOrDefault(item => item is not null)?.Id;
        var requiredResourceCount = marketplaceBookingEntity.Quantity * marketplaceBookingEntity.ProductPricing.NumberOfResourcesToBook;

        foreach (var missingBookingDay in reconciliationPlan.MissingBookingDays)
        {
            var booking = mapper.MapTo(recurringBooking, missingBookingDay);
            booking.Id = randomHelper.Generate();
            if (useOpeningHoursWindow)
            {
                // Closed days are intentionally skipped. Only open location/day combinations can
                // materialize a new generated marketplace booking.
                // The opening-hours service will prefer resource-level overridden availability
                // over the parent location opening hours when selecting the booking window.
                var dailyPlan = await marketplaceBookingOpeningHoursService.TryResolveDailyPlanAsync(
                    customer,
                    marketplaceBookingEntity.ProductVersion,
                    marketplaceBookingEntity.ProductPricing,
                    missingBookingDay,
                    requiredResourceCount,
                    preferredLocationId,
                    cancellationToken);
                if (dailyPlan is null)
                {
                    continue;
                }

                booking.From = dailyPlan.From;
                booking.Until = dailyPlan.Until;
                booking.Schedules = [new BookingSchedule(booking.From, booking.Until)];
                booking.Resources = dailyPlan.Resources
                    .Select(item => new ResourceCustomersPair(new Resource { Id = item.Id }, booking.InvolvedCustomers))
                    .ToList();
            }

            var marketplaceBooking = mapper.MapTo(marketplaceBookingEntity)!;
            marketplaceBooking.Id = randomHelper.Generate();
            marketplaceBooking.IsPaymentRequired = false;
            marketplaceBooking.PaymentStatus = PaymentStatus.NotSet;
            marketplaceBooking.ProductPricing = marketplaceBooking.ProductPricing with
            {
                BookingCadence = useOpeningHoursWindow ? ProductPricingCadence.Daily : marketplaceBooking.ProductPricing.BookingCadence
            };

            booking.MarketplaceBooking = marketplaceBooking;

            await marketplaceBookingService.AddAsync(
                booking,
                recurringBooking.InvolvedCustomers.First(),
                recurringBooking.InvolvedOrganizations,
                recurringBooking.InvolvedTeams,
                recurringBooking,
                cancellationToken);
        }

        return new AdjustRequiredResourcesForMarketplaceRecurringBookingAsyncResponse(false, !reconciliationPlan.HasMoreRequiredBookingDays);
    }

    [Activity]
    public async Task ReleaseMarketplaceRecurringBookingResourcesAsync(ReleaseMarketplaceRecurringBookingResourcesInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var from = new DateTimeOffset(now.UtcDateTime.Date, TimeSpan.Zero);

        var existingBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
            recurringBooking.Id,
            from,
            null,
            cancellationToken);

        foreach (var existingBooking in existingBookings)
        {
            await marketplaceBookingService.DeleteAsync(existingBooking, recurringBooking.DeletedByCustomer, cancellationToken);
        }
    }
}
