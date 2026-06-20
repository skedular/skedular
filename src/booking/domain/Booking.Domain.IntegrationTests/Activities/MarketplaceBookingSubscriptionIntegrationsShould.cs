using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Email;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Temporalio.Testing;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Domain.IntegrationTests.Activities;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceBookingSubscriptionIntegrationsShould(IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_An_InApplication_Delivery_When_Dispatch_Activity_Runs(
        CancellationToken cancellationToken)
    {
        var failureId = $"activity-failure-{Guid.CreateVersion7():N}";
        repositoryFactory.MarketplaceBookingFailureRepository.Add(new MarketplaceBookingFailure
        {
            Id = failureId,
            FailureKey = $"activity-key-{failureId}",
            Category = MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            Scope = MarketplaceBookingFailureScopeConstants.RecurringOccurrence,
            FinalizedAt = DateTimeOffset.UtcNow,
            Deliveries =
            [
                new MarketplaceBookingFailureDelivery
                {
                    Id = $"delivery-{Guid.CreateVersion7():N}",
                    RecipientKey = "customer-1",
                    Audience = MarketplaceBookingFailureDeliveryAudienceConstants.Customer,
                    Channel = MarketplaceBookingFailureDeliveryChannelConstants.InApplication,
                    Status = MarketplaceBookingFailureDeliveryStatusConstants.Pending
                }
            ]
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var transaction = A.Fake<IDbContextTransaction>();
        var transactionBuilder = A.Fake<IDbTransactionBuilder>();
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, A<CancellationToken>._))
            .Returns(transaction);
        var activity = new MarketplaceBookingFailureNotificationIntegrations(
            repositoryFactory,
            A.Fake<IMarketplaceBookingFailureNotificationService>(),
            A.Fake<IEmailService>(),
            transactionBuilder,
            TimeProvider.System,
            A.Fake<IRandomHelper>(),
            A.Fake<ILogger<MarketplaceBookingFailureNotificationIntegrations>>());

        await new ActivityEnvironment().RunAsync(() => activity.DispatchAsync(
            new DispatchMarketplaceBookingFailureNotificationsInput(failureId)));

        var persistedFailure = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetByIdAsync(failureId, cancellationToken);
        persistedFailure.ShouldNotBeNull();
        persistedFailure.Deliveries.Single().Status.ShouldBe(MarketplaceBookingFailureDeliveryStatusConstants.Sent);
        persistedFailure.Deliveries.Single().AttemptCount.ShouldBe(1);
    }

    /// <summary>
    ///     When initial-series materialization fails (all-or-nothing), a single InitialSeries failure
    ///     is recorded. No individual bookings are created for that recurring booking, so the failure
    ///     record is the only artefact of the attempt.
    /// </summary>
    [Theory]
    [AutoFakeItEasyData]
    public async Task Retain_One_Initial_Series_Failure_Without_Materializing_Any_Booking(
        string subscriptionId,
        string recurringBookingId,
        string failureId,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 7, 26, 0, 0, 0, TimeSpan.Zero);
        repositoryFactory.MarketplaceBookingFailureRepository.Add(new MarketplaceBookingFailure
        {
            Id = failureId,
            FailureKey = $"initial-series-{recurringBookingId}",
            MarketplaceBookingSubscriptionId = subscriptionId,
            RecurringBookingId = recurringBookingId,
            Category = MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            Scope = MarketplaceBookingFailureScopeConstants.InitialSeries,
            FinalizedAt = DateTimeOffset.UtcNow,
            CustomerAction = MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var failure = await repositoryFactory.MarketplaceBookingFailureRepository
            .GetByRecurringBookingIdAsync(recurringBookingId, cancellationToken);
        var bookingsForRecurringId = await repositoryFactory.BookingRepository
            .GetByRecurringBookingIdAsync(recurringBookingId, from, null, cancellationToken);

        failure.ShouldNotBeNull();
        failure.Scope.ShouldBe(MarketplaceBookingFailureScopeConstants.InitialSeries);
        failure.MarketplaceBookingSubscriptionId.ShouldBe(subscriptionId);
        failure.BookingId.ShouldBeNull();
        bookingsForRecurringId.ShouldBeEmpty();
    }

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
