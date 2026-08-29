using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Temporalio.Testing;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using MarketplaceBookingFailure = Booking.Shared.Database.Entities.MarketplaceBookingFailure;
using MarketplaceBookingFailureCategoryConstants = Booking.Shared.Models.MarketplaceBookingFailureCategoryConstants;
using MarketplaceBookingFailureFinalization = Booking.Shared.Models.MarketplaceBookingFailureFinalization;
using MarketplaceBookingFailureScopeConstants = Booking.Shared.Models.MarketplaceBookingFailureScopeConstants;
using MarketplaceBookingFailureAccountingCleanupStatusConstants = Booking.Shared.Models.MarketplaceBookingFailureAccountingCleanupStatus;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Activities.MarketplaceBookingSubscriptionIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReleaseRecurringBookingResourcesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Release_Future_Bookings_Before_Cancelling_Recurring_Invoice(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRecurringBookingRepository recurringBookingRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IMarketplaceBookingService marketplaceBookingService,
        [Frozen]
        IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen]
        IMarketplaceBookingFailureService marketplaceBookingFailureService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        string recurringBookingId,
        string bookingId)
    {
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 4, 5, 8, 37, 0, TimeSpan.Zero);
        var from = now.StartOfDay();
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1",
                IsPaymentRequired = true,
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
            },
        };
        var existingBooking = new BookingEntity
        {
            Id = bookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-2",
            },
        };

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure-1",
        };
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(A<MarketplaceBookingFailureFinalization>._,
                environment.CancellationTokenSource.Token))
            .Returns(failure);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                A<DateTimeOffset>._,
                null,
                environment.CancellationTokenSource.Token))
            .Returns(new List<BookingEntity>
            {
                existingBooking,
            });

        await environment.RunAsync(() =>
            sut.ReleaseRecurringBookingResourcesAsync(new ReleaseRecurringBookingResourcesInput(
                recurringBookingId,
                MarketplaceBookingFailureCategoryConstants.PaymentFailed)));

        A.CallTo(() => accountingInvoiceCancellationService.CancelRecurringBookingAsync(recurringBooking, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        recurringBooking.MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatusConstants.RecordNeverCreated);
        A.CallTo(() => marketplaceBookingRepository.Update(recurringBooking.MarketplaceBooking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                from,
                null,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingService.DeleteAsync(existingBooking, null, false, null, false, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappened(2, Times.Exactly);
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>.That.Matches(item =>
                    item.Category == MarketplaceBookingFailureCategoryConstants.PaymentFailed &&
                    item.Scope == MarketplaceBookingFailureScopeConstants.RecurringCycle &&
                    item.RecurringBookingId == recurringBookingId),
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatusConstants.Pending,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatusConstants.NotRequired,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_Terminal_State_When_No_Future_Bookings_Exist(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRecurringBookingRepository recurringBookingRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        string recurringBookingId)
    {
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 4, 5, 8, 37, 0, TimeSpan.Zero);
        var from = now.StartOfDay();
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1",
                IsPaymentRequired = true,
                PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
                InvoiceUrl = "https://example.com/invoice",
            },
        };

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                A<DateTimeOffset>._,
                null,
                environment.CancellationTokenSource.Token))
            .Returns(new List<BookingEntity>());

        await environment.RunAsync(() =>
            sut.ReleaseRecurringBookingResourcesAsync(new ReleaseRecurringBookingResourcesInput(recurringBookingId)));

        recurringBooking.MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatusConstants.Expired);
        A.CallTo(() => accountingInvoiceCancellationService.CancelRecurringBookingAsync(recurringBooking, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                from,
                null,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingRepository.Update(recurringBooking.MarketplaceBooking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappened(2, Times.Exactly);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Local_Release_When_Recurring_Accounting_Cancellation_Fails(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRecurringBookingRepository recurringBookingRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen]
        IMarketplaceBookingFailureService marketplaceBookingFailureService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        string recurringBookingId)
    {
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 4, 5, 8, 37, 0, TimeSpan.Zero);
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1",
            },
        };
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure-1",
        };

        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBookingId, environment.CancellationTokenSource.Token))
            .Returns(recurringBooking);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                A<DateTimeOffset>._,
                null,
                environment.CancellationTokenSource.Token))
            .Returns(new List<BookingEntity>());
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(A<MarketplaceBookingFailureFinalization>._,
                environment.CancellationTokenSource.Token))
            .Returns(failure);
        A.CallTo(() => accountingInvoiceCancellationService.CancelRecurringBookingAsync(
                recurringBooking,
                environment.CancellationTokenSource.Token))
            .ThrowsAsync(new InvalidOperationException("Xero is unavailable."));

        await environment.RunAsync(() => sut.ReleaseRecurringBookingResourcesAsync(
            new ReleaseRecurringBookingResourcesInput(recurringBookingId)));

        A.CallTo(() => marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatusConstants.Pending,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatusConstants.TransitionRequired,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }
}
