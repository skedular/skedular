using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Services;

public interface IPrivateBookingService
{
    Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken);

    Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking booking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken);
}

public class PrivateBookingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    IMapper mapper,
    IBookingCheckoutSessionHelperService bookingCheckoutSessionHelperService,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    ICachedBookingService cachedBookingService,
    IResourceService resourceService,
    IGraphQlTopicEventSender graphQlTopicEventSender) : IPrivateBookingService
{
    public async Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken)
    {
        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        /********************************************************************************************************************/
        // TODO: 20250317 : Morteza: For now, remove all existing resources as part of the transaction to make subsequent resource availability easier to manage.
        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        /********************************************************************************************************************/

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var resources = await resourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
            booking.From,
            booking.Until,
            resourceIds,
            cancellationToken);

        foreach (var resource in resources)
        {
            var matchingResource = booking.Resources.FirstOrDefault(item => item.Resource.Id == resource.Id);
            if (matchingResource is null)
            {
                continue;
            }

            var matchingCustomerEntities = customerEntities.Where(item => matchingResource.Customers.Select(x => x.Id).Contains(item.Id)).ToList();

            foreach (var slot in resource.ResourceBookingSlots)
            {
                foreach (var matchingCustomerEntity in matchingCustomerEntities
                             .Where(matchingCustomerEntity => !slot.Customers.Select(item => item.Id).Contains(matchingCustomerEntity.Id)))
                {
                    slot.Customers.Add(matchingCustomerEntity);
                }
            }

            repositoryFactory.ResourceBookingSlotRepository.UpdateRange(resource.ResourceBookingSlots);
        }

        booking.LineItems = existingBooking.LineItems;
        booking.Channel = existingBooking.Channel.ToBookingChannel();
        booking.IsPaymentRequired = existingBooking.IsPaymentRequired;
        booking.PaymentStatus = existingBooking.PaymentStatus.ToPaymentStatus();
        booking.PaymentMethod = existingBooking.PaymentMethod.ToNullablePaymentMethod();
        booking.InvoiceUrl = existingBooking.InvoiceUrl;
        booking.InvoiceNumber = existingBooking.InvoiceNumber;
        booking.TotalAmountExcludeTax = existingBooking.TotalAmountExcludeTax;
        booking.TaxAmount = existingBooking.TaxAmount;
        booking.TaxRatePercentage = existingBooking.TaxRatePercentage;
        booking.TotalAmount = existingBooking.TotalAmount;
        booking.Currency = existingBooking.Currency;

        var bookingEntity = mapper.MergeTo(
            booking,
            existingBooking,
            customerEntities,
            organizations,
            ResourcesToLocations(resources),
            teams,
            resources,
            null,
            null,
            existingBooking.CreatedByCustomer,
            lastModifiedByCustomer,
            null,
            existingBooking.ProductVersions,
            existingBooking.StripeCheckoutSession);

        bookingEntity = repositoryFactory.BookingRepository.Update(bookingEntity);
        booking = mapper.MapTo(bookingEntity, bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(bookingEntity));

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        return booking;
    }

    public async Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking booking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking);

        booking.DeletedByCustomer = deletedByCustomer;
        booking = repositoryFactory.BookingRepository.Update(booking);
        var deletedBooking = mapper.MapTo(
            repositoryFactory.BookingRepository.Remove(booking),
            bookingCheckoutSessionHelperService.GetBookingPaymentExpiry(booking));

        bookingOutboxPublisher.PublishBookings([deletedBooking], repositoryFactory.UnitOfWork);

        if (booking.IsPaymentRequired)
        {
            if (!deletedBooking.PaymentMethod.HasValue)
            {
                throw new PaymentMethodRequired();
            }

            switch (deletedBooking.PaymentMethod)
            {
                case PaymentMethod.Card:
                    temporalOutboxService.SignalWorkflowPayBookingViaCardDeleteBooking(deletedBooking.Id, repositoryFactory.UnitOfWork);
                    break;

                case PaymentMethod.BankTransfer:
                    temporalOutboxService.SignalWorkflowPayBookingViaBankTransferDeleteBooking(deletedBooking.Id, repositoryFactory.UnitOfWork);
                    break;

                default: throw new ArgumentOutOfRangeException();
            }
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.RemoveByIdAsync(deletedBooking.Id, cancellationToken);

        return deletedBooking;
    }

    private static List<Location> ResourcesToLocations(ICollection<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;
}
