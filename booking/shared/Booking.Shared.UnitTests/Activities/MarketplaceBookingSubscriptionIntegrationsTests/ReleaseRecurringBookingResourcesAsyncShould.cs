using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Temporalio.Testing;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Activities.MarketplaceBookingSubscriptionIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReleaseRecurringBookingResourcesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Recurring_Invoice_Before_Releasing_Future_Bookings(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen] IMarketplaceBookingService marketplaceBookingService,
        [Frozen] IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        string recurringBookingId,
        string bookingId)
    {
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 4, 5, 8, 37, 0, TimeSpan.Zero);
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1", IsPaymentRequired = true, PaymentMethod = PaymentMethod.Card.ToPaymentMethod()
            }
        };
        var existingBooking = new BookingEntity { Id = bookingId, MarketplaceBooking = new MarketplaceBooking { Id = "marketplace-booking-2" } };

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
            .Returns(new List<BookingEntity> { existingBooking });

        await environment.RunAsync(() =>
            sut.ReleaseRecurringBookingResourcesAsync(new ReleaseRecurringBookingResourcesInput(recurringBookingId)));

        A.CallTo(() => accountingInvoiceCancellationService.CancelRecurringBookingAsync(recurringBooking, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        recurringBooking.MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatusConstants.RecordNeverCreated);
        A.CallTo(() => marketplaceBookingRepository.Update(recurringBooking.MarketplaceBooking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                now,
                null,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingService.DeleteAsync(existingBooking, null, false, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_Terminal_State_When_No_Future_Bookings_Exist(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen] IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        string recurringBookingId)
    {
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 4, 5, 8, 37, 0, TimeSpan.Zero);
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1",
                IsPaymentRequired = true,
                PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
                InvoiceUrl = "https://example.com/invoice"
            }
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
                now,
                null,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingRepository.Update(recurringBooking.MarketplaceBooking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }
}
