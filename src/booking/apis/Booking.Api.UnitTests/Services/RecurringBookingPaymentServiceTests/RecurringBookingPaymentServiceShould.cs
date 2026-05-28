using Api.Shared.Services.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Microsoft.EntityFrameworkCore.Storage;
using Constants = Booking.Shared.GraphQL.Constants;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using Customer = Booking.Shared.Database.Entities.Customer;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.UnitTests.Services.RecurringBookingPaymentServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class RecurringBookingPaymentServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task ConfirmPaymentAsync_Updates_Recurring_Booking_And_Raises_Subscription_And_Booking_Events(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        RecurringBookingPaymentService sut,
        CancellationToken cancellationToken) =>
        await AssertUpdatePaymentStatusAsync(
            transactionBuilder,
            repositoryFactory,
            cachedCustomerService,
            organizationAuthorizationService,
            marketplaceBookingRepository,
            recurringBookingRepository,
            bookingRepository,
            temporalOutboxService,
            entityMapper,
            graphQlTopicEventSender,
            unitOfWork,
            transaction,
            sut,
            cancellationToken,
            PaymentMethod.BankTransfer,
            PaymentStatus.Confirmed,
            (service, recurringBookingId, token) => service.ConfirmPaymentAsync(recurringBookingId, token));

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectPaymentAsync_Updates_Recurring_Booking_And_Raises_Subscription_And_Booking_Events(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        RecurringBookingPaymentService sut,
        CancellationToken cancellationToken) =>
        await AssertUpdatePaymentStatusAsync(
            transactionBuilder,
            repositoryFactory,
            cachedCustomerService,
            organizationAuthorizationService,
            marketplaceBookingRepository,
            recurringBookingRepository,
            bookingRepository,
            temporalOutboxService,
            entityMapper,
            graphQlTopicEventSender,
            unitOfWork,
            transaction,
            sut,
            cancellationToken,
            PaymentMethod.Card,
            PaymentStatus.Rejected,
            (service, recurringBookingId, token) => service.RejectPaymentAsync(recurringBookingId, token));

    [Theory]
    [AutoFakeItEasyData]
    public async Task MakePaymentNotRequiredAsync_Updates_Recurring_Booking_And_Raises_Subscription_And_Booking_Events(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen] IRecurringBookingRepository recurringBookingRepository,
        [Frozen] IBookingRepository bookingRepository,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        RecurringBookingPaymentService sut,
        CancellationToken cancellationToken) =>
        await AssertUpdatePaymentStatusAsync(
            transactionBuilder,
            repositoryFactory,
            cachedCustomerService,
            organizationAuthorizationService,
            marketplaceBookingRepository,
            recurringBookingRepository,
            bookingRepository,
            temporalOutboxService,
            entityMapper,
            graphQlTopicEventSender,
            unitOfWork,
            transaction,
            sut,
            cancellationToken,
            PaymentMethod.BankTransfer,
            PaymentStatus.NoPaymentRequired,
            (service, recurringBookingId, token) => service.MakePaymentNotRequiredAsync(recurringBookingId, token));

    private static async Task AssertUpdatePaymentStatusAsync(
        IDbTransactionBuilder transactionBuilder,
        IRepositoryFactory repositoryFactory,
        ICachedCustomerService cachedCustomerService,
        IOrganizationAuthorizationService organizationAuthorizationService,
        IMarketplaceBookingRepository marketplaceBookingRepository,
        IRecurringBookingRepository recurringBookingRepository,
        IBookingRepository bookingRepository,
        ITemporalOutboxService temporalOutboxService,
        IEntityMapper entityMapper,
        IGraphQlTopicEventSender graphQlTopicEventSender,
        IUnitOfWork unitOfWork,
        IDbContextTransaction transaction,
        RecurringBookingPaymentService sut,
        CancellationToken cancellationToken,
        PaymentMethod paymentMethod,
        PaymentStatus expectedPaymentStatus,
        Func<RecurringBookingPaymentService, string, CancellationToken, Task<RecurringBooking>> act)
    {
        var customer = new Customer { Id = "customer-1" };
        var subscription = new MarketplaceBookingSubscription { Id = "subscription-1" };
        var organization = new Organization { Id = "organization-1" };
        var recurringBooking = new Shared.Database.Entities.RecurringBooking
        {
            Id = "recurring-1",
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            StartDate = new DateTimeOffset(2026, 3, 19, 0, 0, 0, TimeSpan.Zero),
            MarketplaceBookingSubscription = subscription,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "marketplace-booking-1",
                PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
                PaymentMethod = paymentMethod.ToPaymentMethod()
            },
            InvolvedOrganizations = [organization]
        };
        IReadOnlyList<BookingEntity> relatedBookings = new List<BookingEntity> { new() { Id = "booking-1" }, new() { Id = "booking-2" } };
        var mappedRecurringBooking = new RecurringBooking { Id = recurringBooking.Id };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns(customer.Id);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.RecurringBookingRepository).Returns(recurringBookingRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => recurringBookingRepository.GetByIdAsync(recurringBooking.Id, cancellationToken)).Returns(recurringBooking);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync(organization.Id, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => bookingRepository.GetByRecurringBookingIdUntrackedAsync(recurringBooking.Id, recurringBooking.StartDate, null,
                cancellationToken))
            .Returns(relatedBookings);
        A.CallTo(() => entityMapper.MapTo(recurringBooking)).Returns(mappedRecurringBooking);

        var result = await act(sut, recurringBooking.Id, cancellationToken);

        result.ShouldBe(mappedRecurringBooking);
        recurringBooking.MarketplaceBooking.PaymentStatus.ShouldBe(expectedPaymentStatus.ToPaymentStatus());
        A.CallTo(() => marketplaceBookingRepository.Update(recurringBooking.MarketplaceBooking)).MustHaveHappenedOnceExactly();
        if (paymentMethod == PaymentMethod.Card)
        {
            A.CallTo(() => temporalOutboxService.SignalWorkflowPayRecurringBookingViaCardSetPaymentStatus(
                    recurringBooking.Id,
                    A<SetPaymentStatusArgs>.That.Matches(item => item.PaymentStatus == expectedPaymentStatus.ToPaymentStatus()),
                    unitOfWork))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => temporalOutboxService.SignalWorkflowPayRecurringBookingViaBankTransferSetPaymentStatus(
                    A<string>._,
                    A<SetPaymentStatusArgs>._,
                    A<IUnitOfWork>._))
                .MustNotHaveHappened();
        }
        else
        {
            A.CallTo(() => temporalOutboxService.SignalWorkflowPayRecurringBookingViaCardSetPaymentStatus(
                    A<string>._,
                    A<SetPaymentStatusArgs>._,
                    A<IUnitOfWork>._))
                .MustNotHaveHappened();

            A.CallTo(() => temporalOutboxService.SignalWorkflowPayRecurringBookingViaBankTransferSetPaymentStatus(
                    recurringBooking.Id,
                    A<SetPaymentStatusArgs>.That.Matches(item => item.PaymentStatus == expectedPaymentStatus.ToPaymentStatus()),
                    unitOfWork))
                .MustHaveHappenedOnceExactly();
        }

        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.MarketplaceBookingSubscriptionTopicName, subscription.Id,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, "booking-1", cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, "booking-2", cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
