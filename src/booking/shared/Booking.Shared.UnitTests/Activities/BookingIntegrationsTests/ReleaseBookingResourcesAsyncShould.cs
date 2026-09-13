using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Temporalio.Testing;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using MarketplaceBookingFailure = Booking.Shared.Database.Entities.MarketplaceBookingFailure;
using MarketplaceBookingFailureCategoryConstants = Booking.Shared.Models.MarketplaceBookingFailureCategoryConstants;
using MarketplaceBookingFailureFinalization = Booking.Shared.Models.MarketplaceBookingFailureFinalization;
using MarketplaceBookingFailureScopeConstants = Booking.Shared.Models.MarketplaceBookingFailureScopeConstants;
using MarketplaceBookingFailureAccountingCleanupStatusConstants = Booking.Shared.Models.MarketplaceBookingFailureAccountingCleanupStatus;

namespace Booking.Shared.UnitTests.Activities.BookingIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReleaseBookingResourcesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Booking_Invoice_And_Expire_Payment_When_Releasing_Booking_Resources(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IDbContextTransaction transaction,
        [Frozen]
        IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen]
        IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
        [Frozen]
        IBookingOutboxPublisher bookingOutboxPublisher,
        [Frozen]
        ICachedBookingService cachedBookingService,
        [Frozen]
        IMarketplaceBookingFailureService marketplaceBookingFailureService,
        [Frozen]
        IMarketplaceRefundService marketplaceRefundService,
        BookingIntegrations sut,
        string bookingId)
    {
        var activityEnvironment = new ActivityEnvironment();
        var booking = new BookingEntity
        {
            Id = bookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1",
                StripeCheckoutSession = null,
                PaymentStatus = PaymentStatusConstants.Confirmed,
            },
        };

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => bookingRepository.GetByIdAsync(bookingId, activityEnvironment.CancellationTokenSource.Token)).Returns(booking);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, activityEnvironment.CancellationTokenSource.Token)).Returns(transaction);
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure-1",
        };
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(A<MarketplaceBookingFailureFinalization>._,
                activityEnvironment.CancellationTokenSource.Token))
            .Returns(failure);
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(
                booking, null, activityEnvironment.CancellationTokenSource.Token, true))
            .Returns(new MarketplaceRefund
            {
                Id = "refund-1",
                Status = "Requested",
            });

        await activityEnvironment.RunAsync(() =>
            sut.ReleaseBookingResourcesAsync(new ReleaseBookingResourcesInput(
                bookingId,
                MarketplaceBookingFailureCategoryConstants.PaymentFailed)));

        booking.MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatusConstants.RecordNeverCreated);
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(booking, activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingOutboxPublisher.PublishBookings(A<IReadOnlyList<Shared.Models.Booking>>._, unitOfWork)).MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingRepository.Update(booking.MarketplaceBooking)).MustHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(activityEnvironment.CancellationTokenSource.Token)).MustHaveHappened(2, Times.Exactly);
        A.CallTo(() => transaction.CommitAsync(activityEnvironment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedBookingService.UpdateByIdAsync(bookingId, activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>.That.Matches(item =>
                    item.Category == MarketplaceBookingFailureCategoryConstants.PaymentFailed &&
                    item.Scope == MarketplaceBookingFailureScopeConstants.OneTimeBooking &&
                    item.BookingId == bookingId),
                activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(
                booking, null, activityEnvironment.CancellationTokenSource.Token, true))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatusConstants.Pending,
                activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatusConstants.NotRequired,
                activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Local_Release_When_Accounting_Cancellation_Fails(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IDbContextTransaction transaction,
        [Frozen]
        IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen]
        IMarketplaceBookingFailureService marketplaceBookingFailureService,
        BookingIntegrations sut,
        string bookingId)
    {
        var activityEnvironment = new ActivityEnvironment();
        var booking = new BookingEntity
        {
            Id = bookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1",
            },
        };
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure-1",
        };

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => bookingRepository.GetByIdAsync(bookingId, activityEnvironment.CancellationTokenSource.Token)).Returns(booking);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, activityEnvironment.CancellationTokenSource.Token)).Returns(transaction);
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(A<MarketplaceBookingFailureFinalization>._,
                activityEnvironment.CancellationTokenSource.Token))
            .Returns(failure);
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(booking, activityEnvironment.CancellationTokenSource.Token))
            .ThrowsAsync(new InvalidOperationException("Xero is unavailable."));

        await activityEnvironment.RunAsync(() => sut.ReleaseBookingResourcesAsync(new ReleaseBookingResourcesInput(bookingId)));

        A.CallTo(() => transaction.CommitAsync(activityEnvironment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatusConstants.Pending,
                activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatusConstants.TransitionRequired,
                activityEnvironment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }
}
