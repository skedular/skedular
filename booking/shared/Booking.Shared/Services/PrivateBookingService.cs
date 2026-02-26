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
    Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken);

    Task<Models.Booking> AddAsync(
        bool runInTransaction,
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken);

    Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer? lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken);

    Task<Models.Booking> UpdateAsync(
        bool runInTransaction,
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer? lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken);

    Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken);

    Task<Models.Booking> DeleteAsync(
        bool runInTransaction,
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken);
}

public class PrivateBookingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IBookingOutboxPublisher bookingOutboxPublisher,
    IMapper mapper,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    ICachedBookingService cachedBookingService,
    IResourceService resourceService,
    IPrivateBookingPreferenceService privateBookingPreferenceService,
    IGraphQlTopicEventSender graphQlTopicEventSender) : IPrivateBookingService
{
    public async Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken) =>
        await AddAsync(true, booking, customer, organizations, teams, recurringBooking, cancellationToken);

    public async Task<Models.Booking> AddAsync(
        bool runInTransaction,
        Models.Booking booking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken)
    {
        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        var resources = await resourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
            booking.From,
            booking.Until,
            resourceIds,
            [],
            cancellationToken);

        var transaction = runInTransaction ? await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken) : null;

        try
        {
            if (booking.InvolvedCustomers.Count == 1)
            {
                if (resources.Count == 0)
                {
                    (organizations, resources) = await privateBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                        customerEntities.First(),
                        booking.From,
                        booking.Until,
                        booking.InvolvedOrganizations
                            .Where(item => !string.IsNullOrWhiteSpace(item.Id))
                            .Select(item => item.Id)
                            .ToList(),
                        booking.InvolvedOrganizations
                            .Where(item => !string.IsNullOrWhiteSpace(item.UniqueAlphanumericName))
                            .Select(item => item.UniqueAlphanumericName!)
                            .ToList(),
                        cancellationToken);
                }
            }

            foreach (var resource in resources)
            {
                var matchingResource = booking.Resources.FirstOrDefault(item => item.Resource.Id == resource.Id);
                if (matchingResource is null)
                {
                    continue;
                }

                var matchingCustomerEntities =
                    customerEntities.Where(item => matchingResource.Customers.Select(x => x.Id).Contains(item.Id)).ToList();

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

            var bookingEntity = mapper.MapTo(
                booking,
                customerEntities,
                organizations,
                ResourcesToLocations(resources),
                teams,
                resources,
                customer,
                null,
                null,
                null,
                recurringBooking);

            bookingEntity.Channel = BookingChannelConstants.Private;

            bookingEntity = repositoryFactory.BookingRepository.Add(bookingEntity);
            booking = mapper.MapTo(bookingEntity);

            bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

            if (runInTransaction)
            {
                await transaction!.CommitAsync(cancellationToken);
            }

            await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

            return booking;
        }
        finally
        {
            if (runInTransaction)
            {
                await transaction!.DisposeAsync();
            }
        }
    }

    public async Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer? lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken) =>
        await UpdateAsync(true, booking, existingBooking, lastModifiedByCustomer, organizations, teams, recurringBooking, cancellationToken);

    public async Task<Models.Booking> UpdateAsync(
        bool runInTransaction,
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer? lastModifiedByCustomer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken)
    {
        if (existingBooking.Channel.ToBookingChannel() != BookingChannel.Private)
        {
            throw new BookingIsNotPrivate();
        }

        var customerIds = booking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var transaction = runInTransaction ? await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken) : null;

        try
        {
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
                [],
                cancellationToken);

            foreach (var resource in resources)
            {
                var matchingResource = booking.Resources.FirstOrDefault(item => item.Resource.Id == resource.Id);
                if (matchingResource is null)
                {
                    continue;
                }

                var matchingCustomerEntities =
                    customerEntities.Where(item => matchingResource.Customers.Select(x => x.Id).Contains(item.Id)).ToList();

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

            var bookingEntity = mapper.MergeTo(
                booking,
                existingBooking,
                customerEntities,
                organizations,
                ResourcesToLocations(resources),
                teams,
                resources,
                existingBooking.CreatedByCustomer,
                lastModifiedByCustomer,
                null,
                null,
                recurringBooking);

            bookingEntity = repositoryFactory.BookingRepository.Update(bookingEntity);
            booking = mapper.MapTo(bookingEntity);

            bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

            if (runInTransaction)
            {
                await transaction!.CommitAsync(cancellationToken);
            }

            await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

            return booking;
        }
        finally
        {
            if (runInTransaction)
            {
                await transaction!.DisposeAsync();
            }
        }
    }

    public async Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken) =>
        await DeleteAsync(true, existingBooking, deletedByCustomer, cancellationToken);

    public async Task<Models.Booking> DeleteAsync(
        bool runInTransaction,
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken)
    {
        if (existingBooking.Channel.ToBookingChannel() != BookingChannel.Private)
        {
            throw new BookingIsNotPrivate();
        }

        var transaction = runInTransaction ? await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken) : null;

        try
        {
            bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);

            existingBooking.DeletedByCustomer = deletedByCustomer;
            existingBooking = repositoryFactory.BookingRepository.Update(existingBooking);
            var deletedBooking = mapper.MapTo(repositoryFactory.BookingRepository.Remove(existingBooking));

            bookingOutboxPublisher.PublishBookings([deletedBooking], repositoryFactory.UnitOfWork);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

            if (runInTransaction)
            {
                await transaction!.CommitAsync(cancellationToken);
            }

            await cachedBookingService.RemoveByIdAsync(deletedBooking.Id, cancellationToken);

            return deletedBooking;
        }
        finally
        {
            if (runInTransaction)
            {
                await transaction!.DisposeAsync();
            }
        }
    }

    private static List<Location> ResourcesToLocations(ICollection<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;
}
