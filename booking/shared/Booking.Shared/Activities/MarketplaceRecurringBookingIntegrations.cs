using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
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

        foreach (var existingBooking in reconciliationPlan.BookingsToRemove)
        {
            await marketplaceBookingService.DeleteAsync(existingBooking, null, cancellationToken);
        }

        var updatedByCustomer = recurringBooking.LastModifiedByCustomer ?? recurringBooking.CreatedByCustomer;
        ArgumentNullException.ThrowIfNull(updatedByCustomer);

        var bookingsToRemoveIds = reconciliationPlan.BookingsToRemove.Select(item => item.Id).ToHashSet();
        var existingBookingsToRefresh = existingBookings
            .Where(item => !bookingsToRemoveIds.Contains(item.Id))
            .Where(item => item.HasRecurringInstanceOverrides != true);

        foreach (var existingBooking in existingBookingsToRefresh)
        {
            await marketplaceBookingService.AdjustRequiredResourcesAsync(existingBooking, cancellationToken);
        }

        foreach (var missingBookingDay in reconciliationPlan.MissingBookingDays)
        {
            var booking = mapper.MapTo(recurringBooking, missingBookingDay);
            booking.Id = randomHelper.Generate();

            var marketplaceBooking = mapper.MapTo(marketplaceBookingEntity)!;
            marketplaceBooking.Id = randomHelper.Generate();
            marketplaceBooking.ProductPricing = marketplaceBooking.ProductPricing with { Cadence = ProductPricingCadence.Daily };
            marketplaceBooking.IsPaymentRequired = false;
            marketplaceBooking.PaymentStatus = PaymentStatus.NotSet;

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
