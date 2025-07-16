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
        var organizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, false, false, cancellationToken);
        if (organizations.Any(organization => !organizationAuthorizationService.CanModifyPaymentMethod(organization, customer)))
        {
            throw new UnauthorizedAccessException();
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
            bookingOutboxPublisher.SignalWorkflowPayBookingViaBankTransferSetPaymentStatus(
                existingBooking.Id,
                new SetPaymentStatusArgs(existingBooking.PaymentStatus),
                repositoryFactory.UnitOfWork);
        }

        var booking = mapper.MapTo(existingBooking, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(existingBooking));

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if ((existingBooking.InvolvedOrganizations.Count == 0 ||
             existingBooking.InvolvedOrganizations.Any(item => !organizationAuthorizationService.CanViewMemberPersonalDetails(item, customer))) &&
            existingBooking.InvolvedOrganizations.Any(item =>
                item.MemberVisibilityPolicy == OrganizationMemberVisibilityPolicyConstants.LimitedAccess))
        {
            booking.InvolvedCustomers = booking.InvolvedCustomers.Select(item =>
            {
                item = item.Redact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                foreach (var identity in item.Identities)
                {
                    identity.Email = identity.Email.FullRedact(OrganizationMemberVisibilityPolicy.LimitedAccess);
                }

                return item;
            }).ToList();
        }

        return booking;
    }
}
