using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Mappers;
using Booking.Api.Services.Authorization;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows.Payment;
using Enterprise.Shared.Database;

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
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ITemporalOutboxPublisher temporalOutboxPublisher,
    IMapper mapper,
    IBookingCheckoutSessionHelperService bookingCheckoutSessionHelperService,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService) : IBookingPaymentService
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
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingBooking = await repositoryFactory.BookingRepository.GetByIdAsync(id, cancellationToken) ?? throw new BookingNotFound();
        if (!existingBooking.BookedOnMarketplace)
        {
            throw new BookingIsNotMarketplaceType();
        }

        var productVersionIds = existingBooking.ProductVersions.Select(item => item.Id).ToList();
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

        existingBooking.PaymentStatus = paymentStatus.ToPaymentStatus();

        if (!string.IsNullOrWhiteSpace(existingBooking.PaymentMethod) &&
            existingBooking.PaymentMethod.ToPaymentMethod() == PaymentMethod.BankTransfer)
        {
            temporalOutboxPublisher.SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(
                existingBooking.Id,
                new SetPaymentStatusArgs(existingBooking.PaymentStatus),
                repositoryFactory.UnitOfWork);
        }

        var booking = mapper.MapTo(existingBooking, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(existingBooking));

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return booking;
    }
}
