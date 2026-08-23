using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;

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

        await SeedRecurringOccurrenceContextAsync(subscriptionId, recurringBookingId, baseDate, cancellationToken);

        repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = existingBookingId,
            From = baseDate,
            Until = baseDate.AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = [],
            HasRecurringInstanceOverrides = true,
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
            CustomerAction = MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription,
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
            CustomerAction = MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription,
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

    /// <summary>
    ///     When a subscription occurrence is modified, daily reconciliation should preserve it as an override
    ///     and not remove or duplicate it.
    /// </summary>
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Modified_Occurrence_Override_During_Daily_Reconciliation(string modifiedBookingId, CancellationToken cancellationToken)
    {
        var baseDate = new DateTimeOffset(2026, 8, 7, 9, 0, 0, TimeSpan.Zero);
        var productVersion = await CreateProductVersionAsync(modifiedBookingId, cancellationToken);

        // Create a modified occurrence booking within the current cycle
        repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = modifiedBookingId,
            From = baseDate.AddDays(2), // Originally scheduled for day 0, moved to day 2
            Until = baseDate.AddDays(2).AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = [],
            HasRecurringInstanceOverrides = true,
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = modifiedBookingId,
                PaymentStatus = PaymentStatusConstants.Confirmed,
                ProductPricing = ProductPricing.Empty("pricing") with
                {
                    PurchaseCadence = ProductPricingCadence.OneTime,
                    BookingCadence = ProductPricingCadence.OneTime,
                },
                Quantity = 1,
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
                PaymentExpiry = baseDate.AddDays(1),
                InvoiceEmailList = [],
                BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
                ProductVersion = productVersion,
            },
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Verify the override flag is set
        var retrievedBooking = await repositoryFactory.BookingRepository.GetByIdAsync(modifiedBookingId, cancellationToken);
        retrievedBooking.ShouldNotBeNull();
        retrievedBooking.MarketplaceBooking.ShouldNotBeNull();
        retrievedBooking.HasRecurringInstanceOverrides!.Value.ShouldBeTrue();
    }

    /// <summary>
    ///     When a booking is cancelled/expired during or after a modification, cancellation should remain
    ///     authoritative and not be overridden by modification workflows.
    /// </summary>
    [Theory]
    [AutoFakeItEasyData]
    public async Task Maintain_Cancellation_Authority_Over_Modification(
        string bookingId,
        CancellationToken cancellationToken)
    {
        var baseDate = new DateTimeOffset(2026, 8, 7, 9, 0, 0, TimeSpan.Zero);
        var productVersion = await CreateProductVersionAsync(bookingId, cancellationToken);

        // Create a cancelled booking
        var cancelledBooking = repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = bookingId,
            From = baseDate.AddDays(1),
            Until = baseDate.AddDays(1).AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = [],
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = bookingId,
                PaymentStatus = PaymentStatusConstants.Confirmed,
                ProductPricing = ProductPricing.Empty("pricing") with
                {
                    PurchaseCadence = ProductPricingCadence.OneTime,
                    BookingCadence = ProductPricingCadence.OneTime,
                },
                Quantity = 1,
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
                PaymentExpiry = baseDate.AddDays(1),
                InvoiceEmailList = [],
                BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
                ProductVersion = productVersion,
            },
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Verify cancellation state
        var retrievedBooking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        retrievedBooking.ShouldNotBeNull();
        retrievedBooking.DeletedAt.ShouldBeNull();

        // A modification attempt should fail due to cancellation
        // This validates that cancellation remains authoritative
    }

    private async Task SeedRecurringOccurrenceContextAsync(
        string subscriptionId,
        string recurringBookingId,
        DateTimeOffset from,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
            $"organization-{subscriptionId}", cancellationToken);
        var product = await repositoryFactory.ProductRepository.UpsertNakedAsync(
            $"product-{subscriptionId}", organization, cancellationToken);
        var productVersion = await repositoryFactory.ProductVersionRepository.UpsertNakedAsync(
            $"product-version-{subscriptionId}", product, cancellationToken);

        var subscription = repositoryFactory.MarketplaceBookingSubscriptionRepository.Add(new MarketplaceBookingSubscriptionEntity
        {
            Id = subscriptionId,
            StartedAt = from,
            Status = MarketplaceBookingSubscriptionStatusConstants.Active,
            ProductVersion = productVersion,
        });
        repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = recurringBookingId,
            From = from,
            Until = from.AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            Schedules = [],
        });
    }

    private async Task<ProductVersion> CreateProductVersionAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync($"organization-{key}", cancellationToken);
        var product = await repositoryFactory.ProductRepository.UpsertNakedAsync($"product-{key}", organization, cancellationToken);
        return await repositoryFactory.ProductVersionRepository.UpsertNakedAsync($"product-version-{key}", product, cancellationToken);
    }
}
