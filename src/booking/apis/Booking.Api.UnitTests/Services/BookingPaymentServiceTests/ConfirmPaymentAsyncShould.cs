using Api.Shared.Services.Models;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Microsoft.EntityFrameworkCore.Storage;
using Constants = Booking.Shared.GraphQL.Constants;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Api.UnitTests.Services.BookingPaymentServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ConfirmPaymentAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_The_Marketplace_Booking_And_Raise_The_Booking_Event(
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IBookingOutboxPublisher bookingOutboxPublisher,
        [Frozen]
        ITemporalOutboxService temporalOutboxService,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        IGraphQlTopicEventSender graphQlTopicEventSender,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IProductVersionRepository productVersionRepository,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IMarketplaceBookingRepository marketplaceBookingRepository,
        [Frozen]
        IMarketplacePurchaseHistoryRepository marketplacePurchaseHistoryRepository,
        BookingPaymentService sut,
        string bookingId,
        string customerId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = new MarketplaceBookingEntity
        {
            Id = bookingId,
            ProductVersion = new ProductVersionEntity
            {
                Id = "product-version-1",
            },
            PaymentMethod = PaymentMethod.BankTransfer.ToPaymentMethod(),
            PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
            ProductPricing = ProductPricing.Empty("entitlement-pricing") with
            {
                FulfillmentType = ProductPricingFulfillmentType.Entitlement,
                EntitlementCreditQuantity = 2,
                EntitlementValidityDays = 30,
            },
        };
        var existingBooking = new BookingEntity
        {
            Id = bookingId,
            Channel = BookingChannel.Marketplace.ToBookingChannel(),
            MarketplaceBooking = marketplaceBooking,
            InvolvedCustomers =
            [
                new Customer
                {
                    Id = customerId,
                },
            ],
        };
        var productVersion = new ProductVersionEntity
        {
            Id = marketplaceBooking.ProductVersion.Id,
            Product = new ProductEntity
            {
                Organization = new OrganizationEntity
                {
                    Id = organizationId,
                },
            },
        };
        var organization = new OrganizationEntity
        {
            Id = organizationId,
        };
        var mappedBooking = new Shared.Models.Booking
        {
            Id = bookingId,
        };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns(customerId);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingRepository).Returns(marketplaceBookingRepository);
        A.CallTo(() => repositoryFactory.MarketplacePurchaseHistoryRepository).Returns(marketplacePurchaseHistoryRepository);
        A.CallTo(() => bookingRepository.GetByIdAsync(bookingId, cancellationToken)).Returns(existingBooking);
        A.CallTo(() => productVersionRepository.GetByIdAsync(productVersion.Id, cancellationToken)).Returns(productVersion);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organizationId, null, false, false, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync(organizationId, customerId, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => entityMapper.MapTo(existingBooking)).Returns(mappedBooking);

        var result = await sut.ConfirmPaymentAsync(bookingId, cancellationToken);

        result.ShouldBe(mappedBooking);
        marketplaceBooking.PaymentStatus.ShouldBe(PaymentStatus.Confirmed.ToPaymentStatus());
        A.CallTo(() => temporalOutboxService.SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(
                bookingId,
                A<SetPaymentStatusArgs>.That.Matches(item => item.PaymentStatus == PaymentStatus.Confirmed.ToPaymentStatus()),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingRepository.Update(marketplaceBooking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
                marketplaceBooking.Id, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingOutboxPublisher.PublishBookings(
                A<IReadOnlyList<Shared.Models.Booking>>.That.Matches(items => items.Count == 1 && items.Single().Id == bookingId),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, bookingId, cancellationToken))
            .MustHaveHappenedOnceExactly();

        // Entitlement grants belong to the standalone purchase flow, never to a booking payment transition.
        A.CallTo(() => repositoryFactory.EntitlementPurchaseRepository).MustNotHaveHappened();
    }
}
