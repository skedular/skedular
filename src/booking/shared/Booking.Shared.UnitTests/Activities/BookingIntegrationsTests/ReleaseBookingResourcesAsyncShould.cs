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

namespace Booking.Shared.UnitTests.Activities.BookingIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReleaseBookingResourcesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Booking_Invoice_And_Expire_Payment_When_Releasing_Booking_Resources(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen] IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
        [Frozen] IBookingOutboxPublisher bookingOutboxPublisher,
        [Frozen] ICachedBookingService cachedBookingService,
        [Frozen] IMarketplaceBookingFailureService marketplaceBookingFailureService,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        BookingIntegrations sut,
        string bookingId)
    {
        var environment = new ActivityEnvironment();
        var booking = new BookingEntity
        {
            Id = bookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1", StripeCheckoutSession = null, PaymentStatus = PaymentStatusConstants.Confirmed
            }
        };

        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => bookingRepository.GetByIdAsync(bookingId, environment.CancellationTokenSource.Token)).Returns(booking);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, environment.CancellationTokenSource.Token)).Returns(transaction);
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(A<MarketplaceBookingFailureFinalization>._,
                environment.CancellationTokenSource.Token))
            .Returns(new MarketplaceBookingFailure());
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(
                booking, null, environment.CancellationTokenSource.Token, true))
            .Returns(new MarketplaceRefund { Id = "refund-1", Status = "Requested" });

        await environment.RunAsync(() =>
            sut.ReleaseBookingResourcesAsync(new ReleaseBookingResourcesInput(
                bookingId,
                MarketplaceBookingFailureCategoryConstants.PaymentFailed)));

        booking.MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatusConstants.RecordNeverCreated);
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(booking, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingOutboxPublisher.PublishBookings(A<IReadOnlyList<Shared.Models.Booking>>._, unitOfWork)).MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingRepository.Update(booking.MarketplaceBooking)).MustHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedTwiceExactly();
        A.CallTo(() => transaction.CommitAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedBookingService.UpdateByIdAsync(bookingId, environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingFailureService.FinalizeAsync(
                A<MarketplaceBookingFailureFinalization>.That.Matches(item =>
                    item.Category == MarketplaceBookingFailureCategoryConstants.PaymentFailed &&
                    item.Scope == MarketplaceBookingFailureScopeConstants.OneTimeBooking &&
                    item.BookingId == bookingId),
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceRefundService.CreateBookingCancellationRefundAsync(
                booking, null, environment.CancellationTokenSource.Token, true))
            .MustHaveHappenedOnceExactly();
    }
}
