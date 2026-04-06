using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Grpc.Core;
using Microsoft.EntityFrameworkCore.Storage;
using Temporalio.Testing;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;

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
        [Frozen] IMapper mapper,
        OrganizationConfiguration organizationConfiguration,
        CallInvoker callInvoker,
        IOrganizationArrearsBillingPlannerService organizationArrearsBillingPlannerService,
        IGraphQlTopicEventSender graphQlTopicEventSender,
        string bookingId)
    {
        var environment = new ActivityEnvironment();
        var sut = new BookingIntegrations(
            organizationConfiguration,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            transactionBuilder,
            bookingResourceSlotsHelperService,
            mapper,
            organizationArrearsBillingPlannerService,
            bookingOutboxPublisher,
            cachedBookingService,
            graphQlTopicEventSender,
            accountingInvoiceCancellationService);
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

        await environment.RunAsync(() =>
            sut.ReleaseBookingResourcesAsync(new ReleaseBookingResourcesInput(bookingId)));

        booking.MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatusConstants.RecordNeverCreated);
        A.CallTo(() => accountingInvoiceCancellationService.CancelBookingAsync(booking, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking)).MustHaveHappenedOnceExactly();
        A.CallTo(() => bookingOutboxPublisher.PublishBookings(A<ICollection<Models.Booking>>._, unitOfWork)).MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingRepository.Update(booking.MarketplaceBooking)).MustHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedTwiceExactly();
        A.CallTo(() => transaction.CommitAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedBookingService.UpdateByIdAsync(bookingId, environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
    }
}
