using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Temporalio.Testing;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Activities.MarketplaceBookingCleanupIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class EnqueueAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Finalize_And_Enqueue_A_Missing_One_Time_Booking_Failure(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IMarketplaceBookingFailureService marketplaceBookingFailureService,
        [Frozen]
        ITemporalOutboxService temporalOutboxService,
        [Frozen]
        IUnitOfWork unitOfWork,
        MarketplaceBookingCleanupIntegrations sut,
        string bookingId,
        string failureId,
        DateTimeOffset from,
        DateTimeOffset until)
    {
        var input = new EnqueueMarketplaceBookingCleanupInput(
            bookingId,
            null,
            FailureCategory: MarketplaceBookingFailureCategoryConstants.PaymentExpired);
        var booking = new BookingEntity
        {
            Id = bookingId,
            From = from,
            Until = until,
            MarketplaceBooking = new MarketplaceBookingEntity(),
        };
        var failure = new MarketplaceBookingFailure
        {
            Id = failureId,
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => failureRepository.GetByBookingIdAsync(bookingId, A<CancellationToken>._))
            .Returns((MarketplaceBookingFailure?)null);
        A.CallTo(() => bookingRepository.GetByIdAsync(bookingId, A<CancellationToken>._)).Returns(booking);
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>._,
                A<CancellationToken>._))
            .Returns(failure);
        var environment = new ActivityEnvironment();

        await environment.RunAsync(() => sut.EnqueueAsync(input));

        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>.That.Matches(item =>
                    item.BookingId == bookingId &&
                    item.RecurringBookingId == null &&
                    item.Category == MarketplaceBookingFailureCategoryConstants.PaymentExpired &&
                    item.Scope == MarketplaceBookingFailureScopeConstants.OneTimeBooking &&
                    item.RequestedFrom == from &&
                    item.RequestedUntil == until),
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalOutboxService.StartWorkflowMarketplaceBookingCleanup(
                new MarketplaceBookingCleanupInput(failureId), unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Finalize_And_Enqueue_A_Missing_Recurring_Booking_Failure(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        [Frozen]
        IRecurringBookingRepository recurringBookingRepository,
        [Frozen]
        IMarketplaceBookingFailureService marketplaceBookingFailureService,
        [Frozen]
        ITemporalOutboxService temporalOutboxService,
        [Frozen]
        IUnitOfWork unitOfWork,
        MarketplaceBookingCleanupIntegrations sut,
        string recurringBookingId,
        string subscriptionId,
        string failureId,
        DateTimeOffset from,
        DateTimeOffset until)
    {
        var input = new EnqueueMarketplaceBookingCleanupInput(
            null,
            recurringBookingId,
            FailureCategory: MarketplaceBookingFailureCategoryConstants.PaymentExpired);
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            StartDate = from,
            EndDate = until,
            MarketplaceBookingSubscription = new MarketplaceBookingSubscriptionEntity
            {
                Id = subscriptionId,
            },
        };
        var failure = new MarketplaceBookingFailure
        {
            Id = failureId,
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => failureRepository.GetByRecurringBookingIdAsync(recurringBookingId, A<CancellationToken>._))
            .Returns((MarketplaceBookingFailure?)null);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, A<CancellationToken>._)).Returns(recurringBooking);
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>._,
                A<CancellationToken>._))
            .Returns(failure);
        var environment = new ActivityEnvironment();

        await environment.RunAsync(() => sut.EnqueueAsync(input));

        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>.That.Matches(item =>
                    item.BookingId == null &&
                    item.RecurringBookingId == recurringBookingId &&
                    item.MarketplaceBookingSubscriptionId == subscriptionId &&
                    item.Category == MarketplaceBookingFailureCategoryConstants.PaymentExpired &&
                    item.Scope == MarketplaceBookingFailureScopeConstants.RecurringCycle &&
                    item.RequestedFrom == from &&
                    item.RequestedUntil == until),
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalOutboxService.StartWorkflowMarketplaceBookingCleanup(
                new MarketplaceBookingCleanupInput(failureId), unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }
}
