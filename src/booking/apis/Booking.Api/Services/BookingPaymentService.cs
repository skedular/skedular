using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Api.Services;

public interface IBookingPaymentService
{
    Task<Shared.Models.Booking> ConfirmPaymentAsync(string id, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> RejectPaymentAsync(string id, CancellationToken cancellationToken);
    Task<Shared.Models.Booking> MakePaymentNotRequiredAsync(string id, CancellationToken cancellationToken);
}

public class BookingPaymentService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IEntityMapper entityMapper,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    ILogger<BookingPaymentService> logger) : IBookingPaymentService
{
    public async Task<Shared.Models.Booking> ConfirmPaymentAsync(string id, CancellationToken cancellationToken) =>
        await UpdatePaymentStatusInternalAsync(id, PaymentStatus.Confirmed, false, cancellationToken);

    public async Task<Shared.Models.Booking> RejectPaymentAsync(string id, CancellationToken cancellationToken) =>
        await UpdatePaymentStatusInternalAsync(id, PaymentStatus.Rejected, true, cancellationToken);

    public async Task<Shared.Models.Booking> MakePaymentNotRequiredAsync(string id, CancellationToken cancellationToken) =>
        await UpdatePaymentStatusInternalAsync(id, PaymentStatus.NoPaymentRequired, false, cancellationToken);

    private async Task<Shared.Models.Booking> UpdatePaymentStatusInternalAsync(
        string id,
        PaymentStatus paymentStatus,
        bool releaseResources,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        logger.LogInformation(
            "Updating marketplace booking payment status. BookingId={BookingId}, CustomerId={CustomerId}, PaymentStatus={PaymentStatus}, ReleaseResources={ReleaseResources}",
            id,
            customerId,
            paymentStatus,
            releaseResources);
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();
        var marketplaceBooking = existingBooking.MarketplaceBooking;
        if (existingBooking.Channel.ToBookingChannel() != BookingChannel.Marketplace || marketplaceBooking is null)
        {
            throw new BookingIsNotMarketplaceType();
        }

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            productVersion.Product.Organization.Id,
            null,
            false,
            false,
            cancellationToken) ?? throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(organization.Id, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (releaseResources)
        {
            bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);
        }

        marketplaceBooking.PaymentStatus = paymentStatus.ToPaymentStatus();

        switch (marketplaceBooking.PaymentMethod.ToPaymentMethod())
        {
            case PaymentMethod.Card:
                temporalOutboxService.SignalWorkflowPayBookingViaCardSetPaymentStatus(
                    existingBooking.Id,
                    new SetPaymentStatusArgs(marketplaceBooking.PaymentStatus),
                    repositoryFactory.UnitOfWork);
                break;

            case PaymentMethod.BankTransfer:
                temporalOutboxService.SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(
                    existingBooking.Id,
                    new SetPaymentStatusArgs(marketplaceBooking.PaymentStatus),
                    repositoryFactory.UnitOfWork);
                break;

            default:
                throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case.");
        }

        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);

        var booking = entityMapper.MapTo(existingBooking);

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        logger.LogInformation(
            "Updated marketplace booking payment status. BookingId={BookingId}, PaymentStatus={PaymentStatus}, PaymentMethod={PaymentMethod}",
            booking.Id,
            paymentStatus,
            marketplaceBooking.PaymentMethod);

        return booking;
    }
}
