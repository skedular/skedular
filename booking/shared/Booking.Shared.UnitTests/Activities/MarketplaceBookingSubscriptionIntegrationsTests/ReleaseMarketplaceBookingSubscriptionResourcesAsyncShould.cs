using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Temporalio.Testing;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Activities.MarketplaceBookingSubscriptionIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ReleaseMarketplaceBookingSubscriptionResourcesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Stop_Recurring_Billing_Cancel_Recurring_Invoice_And_Delete_Recurring_Booking_When_Subscription_Is_Deleted(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IMarketplaceBookingService marketplaceBookingService,
        [Frozen] ITemporalService temporalService,
        [Frozen] IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        string subscriptionId,
        string recurringBookingId,
        string bookingId)
    {
        var environment = new ActivityEnvironment();
        var deletedByCustomer = new Customer { Id = "customer-1" };
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking { IsPaymentRequired = true, PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod() }
        };
        var existingBooking = new BookingEntity { Id = bookingId, MarketplaceBooking = new MarketplaceBooking { Id = "marketplace-booking-1" } };
        var subscription = new MarketplaceBookingSubscription
        {
            Id = subscriptionId, DeletedByCustomer = deletedByCustomer, RecurringBookings = [recurringBooking]
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 4, 5, 8, 0, 0, TimeSpan.Zero));
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync(subscriptionId, environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                A<DateTimeOffset>._,
                null,
                environment.CancellationTokenSource.Token))
            .Returns(new List<BookingEntity> { existingBooking });
        A.CallTo(() => recurringBookingRepository.Update(recurringBooking)).Returns(recurringBooking);
        A.CallTo(() => recurringBookingRepository.Remove(recurringBooking)).Returns(recurringBooking);

        await environment.RunAsync(() =>
            sut.ReleaseMarketplaceBookingSubscriptionResourcesAsync(new ReleaseMarketplaceBookingSubscriptionResourcesInput(subscriptionId)));

        A.CallTo(() => temporalService.SignalPayRecurringBookingViaBankTransferWorkflowDeleteRecurringBookingAsync(
                recurringBookingId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceCancellationService.CancelRecurringBookingAsync(recurringBooking, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingService.DeleteAsync(existingBooking, deletedByCustomer, false, false,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => recurringBookingRepository.Remove(recurringBooking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Release_Only_Bookings_From_Today_Forward_When_Subscription_Is_Deleted(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] ITemporalService temporalService,
        [Frozen] IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionIntegrations sut,
        string subscriptionId,
        string recurringBookingId)
    {
        var environment = new ActivityEnvironment();
        var deletedByCustomer = new Customer { Id = "customer-1" };
        var now = new DateTimeOffset(2026, 4, 5, 8, 37, 0, TimeSpan.Zero);
        var from = now.StartOfDay();
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking { IsPaymentRequired = true, PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod() }
        };
        var subscription = new MarketplaceBookingSubscription
        {
            Id = subscriptionId, DeletedByCustomer = deletedByCustomer, RecurringBookings = [recurringBooking]
        };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync(subscriptionId, environment.CancellationTokenSource.Token))
            .Returns(subscription);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                A<DateTimeOffset>._,
                null,
                environment.CancellationTokenSource.Token))
            .Returns(new List<BookingEntity>());
        A.CallTo(() => recurringBookingRepository.Update(recurringBooking)).Returns(recurringBooking);
        A.CallTo(() => recurringBookingRepository.Remove(recurringBooking)).Returns(recurringBooking);

        await environment.RunAsync(() =>
            sut.ReleaseMarketplaceBookingSubscriptionResourcesAsync(new ReleaseMarketplaceBookingSubscriptionResourcesInput(subscriptionId)));

        A.CallTo(() => bookingRepository.GetByRecurringBookingIdAsync(
                recurringBookingId,
                from,
                null,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalService.SignalPayRecurringBookingViaBankTransferWorkflowDeleteRecurringBookingAsync(
                recurringBookingId,
                environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceCancellationService.CancelRecurringBookingAsync(recurringBooking, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }
}
