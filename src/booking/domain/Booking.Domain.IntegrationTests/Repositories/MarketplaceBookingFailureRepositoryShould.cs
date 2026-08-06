using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Resource = Booking.Shared.Database.Entities.Resource;
using ResourceBookingSlot = Booking.Shared.Database.Entities.ResourceBookingSlot;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceBookingFailureRepositoryShould(IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_The_Latest_Failure_For_A_Recurring_Booking_And_Subscription(
        string subscriptionId,
        string recurringBookingId,
        string firstFailureId,
        string latestFailureId,
        CancellationToken cancellationToken)
    {
        var firstFinalizedAt = new DateTimeOffset(2026, 7, 24, 10, 0, 0, TimeSpan.Zero);
        var latestFinalizedAt = firstFinalizedAt.AddMinutes(5);

        repositoryFactory.MarketplaceBookingFailureRepository.Add(new MarketplaceBookingFailure
        {
            Id = firstFailureId,
            FailureKey = $"occurrence-{recurringBookingId}-first",
            MarketplaceBookingSubscriptionId = subscriptionId,
            RecurringBookingId = recurringBookingId,
            Category = MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            Scope = MarketplaceBookingFailureScopeConstants.RecurringOccurrence,
            FinalizedAt = firstFinalizedAt,
        });
        repositoryFactory.MarketplaceBookingFailureRepository.Add(new MarketplaceBookingFailure
        {
            Id = latestFailureId,
            FailureKey = $"occurrence-{recurringBookingId}-latest",
            MarketplaceBookingSubscriptionId = subscriptionId,
            RecurringBookingId = recurringBookingId,
            Category = MarketplaceBookingFailureCategoryConstants.PaymentFailed,
            Scope = MarketplaceBookingFailureScopeConstants.RecurringOccurrence,
            FinalizedAt = latestFinalizedAt,
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var byRecurringBooking = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetByRecurringBookingIdAsync(recurringBookingId, cancellationToken);
        var bySubscription = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetByMarketplaceBookingSubscriptionIdAsync(subscriptionId, cancellationToken);

        byRecurringBooking.ShouldNotBeNull();
        byRecurringBooking.Id.ShouldBe(latestFailureId);
        byRecurringBooking.FinalizedAt.ShouldBe(latestFinalizedAt);
        bySubscription.ShouldNotBeNull();
        bySubscription.Id.ShouldBe(latestFailureId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Null_When_No_Failure_Matches_The_Recurring_Booking_Or_Subscription(
        string recurringBookingId,
        string subscriptionId,
        CancellationToken cancellationToken)
    {
        var byRecurringBooking = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetByRecurringBookingIdAsync(recurringBookingId, cancellationToken);
        var bySubscription = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetByMarketplaceBookingSubscriptionIdAsync(subscriptionId, cancellationToken);

        byRecurringBooking.ShouldBeNull();
        bySubscription.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Enforce_One_Delivery_Per_Failure_Recipient_And_Channel(
        string failureId,
        string recipientKey,
        CancellationToken cancellationToken)
    {
        var failure = repositoryFactory.MarketplaceBookingFailureRepository.Add(new MarketplaceBookingFailure
        {
            Id = failureId,
            FailureKey = $"failure-key-{failureId}",
            Category = MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            Scope = MarketplaceBookingFailureScopeConstants.OneTimeBooking,
            FinalizedAt = TimeProvider.System.GetUtcNow(),
        });
        repositoryFactory.MarketplaceBookingFailureDeliveryRepository.Add(CreateDelivery(failure.Id, recipientKey));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        repositoryFactory.MarketplaceBookingFailureDeliveryRepository.Add(CreateDelivery(failure.Id, recipientKey));

        await Should.ThrowAsync<DbUpdateException>(() => repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Claim_The_Complete_Available_Slot_Set_And_Reject_A_Second_Claim(
        string resourceId,
        string firstBookingId,
        string secondBookingId,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 7, 22, 9, 0, 0, TimeSpan.Zero);
        await SeedResourceSlotAsync(resourceId, from, cancellationToken);
        var firstBooking = await AddBookingAsync(firstBookingId, from, cancellationToken);
        var secondBooking = await AddBookingAsync(secondBookingId, from, cancellationToken);

        var firstClaim = await repositoryFactory.ResourceRepository.TryClaimCompleteSlotSetAsync(
            firstBooking,
            [resourceId],
            cancellationToken);
        var secondClaim = await repositoryFactory.ResourceRepository.TryClaimCompleteSlotSetAsync(
            secondBooking,
            [resourceId],
            cancellationToken);

        firstClaim.Claimed.ShouldBeTrue();
        secondClaim.Claimed.ShouldBeFalse();
        secondClaim.UnavailableResourceIds.ShouldContain(resourceId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_A_Claim_When_Resource_Has_No_Available_Slots(
        string resourceId,
        string bookingId,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 7, 24, 9, 0, 0, TimeSpan.Zero);
        // Seed the resource but mark the slot unavailable
        var resource = repositoryFactory.ResourceRepository.Add(new Resource
        {
            Id = resourceId,
            Capacity = 1,
        });
        repositoryFactory.ResourceBookingSlotRepository.AddRange(
        [
            new ResourceBookingSlot
            {
                Id = $"{resourceId}-slot",
                Resource = resource,
                ResourceId = resourceId,
                Start = from,
                Available = false, // slot marked unavailable
            },
        ]);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        var booking = await AddBookingAsync(bookingId, from, cancellationToken);

        var claim = await repositoryFactory.ResourceRepository.TryClaimCompleteSlotSetAsync(booking, [resourceId], cancellationToken);

        claim.Claimed.ShouldBeFalse();
        claim.UnavailableResourceIds.ShouldContain(resourceId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Release_A_Claimed_Slot_Set_For_A_Booking(
        string resourceId,
        string bookingId,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 7, 23, 9, 0, 0, TimeSpan.Zero);
        await SeedResourceSlotAsync(resourceId, from, cancellationToken);
        var booking = await AddBookingAsync(bookingId, from, cancellationToken);
        var claim = await repositoryFactory.ResourceRepository.TryClaimCompleteSlotSetAsync(booking, [resourceId], cancellationToken);

        await repositoryFactory.ResourceRepository.ReleaseClaimAsync(bookingId, cancellationToken);
        var replacement = await AddBookingAsync($"{bookingId}-replacement", from, cancellationToken);
        var replacementClaim = await repositoryFactory.ResourceRepository.TryClaimCompleteSlotSetAsync(replacement, [resourceId], cancellationToken);

        claim.Claimed.ShouldBeTrue();
        replacementClaim.Claimed.ShouldBeTrue();
    }

    private async Task SeedResourceSlotAsync(string resourceId, DateTimeOffset from, CancellationToken cancellationToken)
    {
        var resource = repositoryFactory.ResourceRepository.Add(new Resource
        {
            Id = resourceId,
            Capacity = 1,
        });
        repositoryFactory.ResourceBookingSlotRepository.AddRange(
        [
            new ResourceBookingSlot
            {
                Id = $"{resourceId}-slot",
                Resource = resource,
                ResourceId = resourceId,
                Start = from,
                Available = true,
            },
        ]);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Shared.Database.Entities.Booking> AddBookingAsync(
        string bookingId,
        DateTimeOffset from,
        CancellationToken cancellationToken)
    {
        var booking = repositoryFactory.BookingRepository.Add(new Shared.Database.Entities.Booking
        {
            Id = bookingId,
            From = from,
            Until = from.AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = [],
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return booking;
    }

    private static MarketplaceBookingFailureDelivery CreateDelivery(string failureId, string recipientKey) => new()
    {
        Id = Guid.CreateVersion7().ToString("N"),
        MarketplaceBookingFailureId = failureId,
        RecipientKey = recipientKey,
        Audience = MarketplaceBookingFailureDeliveryAudienceConstants.Customer,
        Channel = MarketplaceBookingFailureDeliveryChannelConstants.Email,
        Status = MarketplaceBookingFailureDeliveryStatusConstants.Pending,
    };
}
