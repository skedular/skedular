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
    IMapper mapper,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    IGraphQlTopicEventSender graphQlTopicEventSender) : IBookingPaymentService
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
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();
        var marketplaceBooking = existingBooking.MarketplaceBooking;
        if (existingBooking.Channel.ToBookingChannel() != BookingChannel.Marketplace || marketplaceBooking is null)
        {
            throw new BookingIsNotMarketplaceType();
        }

        var productVersionIds = marketplaceBooking.ProductVersions.Select(item => item.Id).ToList();
        var productVersions = await repositoryFactory.ProductVersionRepository.GetByIdsAsync(productVersionIds, cancellationToken);
        var organizationIds = productVersions.Select(item => item.Product.Organization.Id).ToList();
        var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
            organizationIds,
            null,
            false,
            false,
            cancellationToken);
        foreach (var organization in organizations)
        {
            if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(organization.Id, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
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
                throw new ArgumentOutOfRangeException();
        }

        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);

        var booking = mapper.MapTo(existingBooking);

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        return booking;
    }
}
