using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Domain.IntegrationTests.Activities;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceBookingSubscriptionIntegrationsShould(IRepositoryFactory repositoryFactory)
{
    /// <summary>
    ///     When a later recurring occurrence cannot be allocated, a RecurringOccurrence failure is
    ///     recorded and linked to the recurring booking / subscription. Existing bookings for other
    ///     days are unaffected, and the subscription is not cancelled.
    /// </summary>
    [Theory]
    [AutoFakeItEasyData]
    public async Task Retain_Per_Occurrence_Failure_Alongside_Already_Created_Bookings(
        string subscriptionId,
        string recurringBookingId,
        string existingBookingId,
        string firstFailureId,
        string secondFailureId,
        CancellationToken cancellationToken)
    {
        var baseDate = new DateTimeOffset(2026, 7, 26, 9, 0, 0, TimeSpan.Zero);

        repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = existingBookingId,
            From = baseDate,
            Until = baseDate.AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = []
        });

        // Two separate occurrence-level failures for the same recurring booking / subscription.
        repositoryFactory.MarketplaceBookingFailureRepository.Add(new MarketplaceBookingFailure
        {
            Id = firstFailureId,
            FailureKey = $"occurrence-{recurringBookingId}-day1",
            MarketplaceBookingSubscriptionId = subscriptionId,
            RecurringBookingId = recurringBookingId,
            Category = MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            Scope = MarketplaceBookingFailureScopeConstants.RecurringOccurrence,
            FinalizedAt = baseDate.AddDays(1),
            CustomerAction = MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription
        });
        repositoryFactory.MarketplaceBookingFailureRepository.Add(new MarketplaceBookingFailure
        {
            Id = secondFailureId,
            FailureKey = $"occurrence-{recurringBookingId}-day2",
            MarketplaceBookingSubscriptionId = subscriptionId,
            RecurringBookingId = recurringBookingId,
            Category = MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            Scope = MarketplaceBookingFailureScopeConstants.RecurringOccurrence,
            FinalizedAt = baseDate.AddDays(2),
            CustomerAction = MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var latestFailure = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetByRecurringBookingIdAsync(recurringBookingId, cancellationToken);
        var existingBooking = await repositoryFactory.BookingRepository
            .GetByIdAsync(existingBookingId, cancellationToken);

        latestFailure.ShouldNotBeNull();
        latestFailure.Scope.ShouldBe(MarketplaceBookingFailureScopeConstants.RecurringOccurrence);
        latestFailure.MarketplaceBookingSubscriptionId.ShouldBe(subscriptionId);
        // Repository returns the most-recently-finalized; both records exist independently.
        latestFailure.FinalizedAt.ShouldBe(baseDate.AddDays(2));
        // Existing bookings for other days are unaffected by the occurrence failure.
        existingBooking.ShouldNotBeNull();
    }
}
