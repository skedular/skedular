using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using RecurringBooking = Booking.Shared.Models.RecurringBooking;

namespace Booking.Shared.Services;

public interface IPrivateRecurringBookingService
{
    Task<RecurringBooking> AddAsync(
        RecurringBooking recurringBooking,
        Customer customer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken);

    Task<RecurringBooking> UpdateAsync(
        RecurringBooking recurringBooking,
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer lastModifiedByCustomer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken);

    Task<RecurringBooking> DeleteAsync(
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken);
}

public class PrivateRecurringBookingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    ITemporalOutboxService temporalOutboxService) : IPrivateRecurringBookingService
{
    public async Task<RecurringBooking> AddAsync(
        RecurringBooking recurringBooking,
        Customer customer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken)
    {
        var customerIds = recurringBooking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var requestedResourceIds = recurringBooking.RequestedResources.Select(item => item.Id).Distinct().ToList();
        var resourceEntities = requestedResourceIds.Count == 0
            ? []
            : await repositoryFactory.ResourceRepository.GetByIdsAsync(requestedResourceIds, false, cancellationToken);
        if (resourceEntities.Count != requestedResourceIds.Count)
        {
            throw new ResourceNotFound();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var recurringBookingEntity = mapper.MapTo(
            recurringBooking,
            customerEntities,
            organizations,
            teams,
            resourceEntities,
            customer,
            null,
            null,
            null);

        recurringBookingEntity.Channel = BookingChannelConstants.Private;

        recurringBookingEntity = repositoryFactory.RecurringBookingRepository.Add(recurringBookingEntity);
        recurringBooking = mapper.MapTo(recurringBookingEntity);

        temporalOutboxService.StartBookPrivateRecurringResources(
            new BookPrivateRecurringResourcesInput(recurringBooking.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return recurringBooking;
    }

    public async Task<RecurringBooking> UpdateAsync(
        RecurringBooking recurringBooking,
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer lastModifiedByCustomer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken)
    {
        if (existingRecurringBooking.Channel.ToBookingChannel() != BookingChannel.Private)
        {
            throw new RecurringBookingIsNotPrivate();
        }

        var customerIds = recurringBooking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var requestedResourceIds = recurringBooking.RequestedResources.Select(item => item.Id).Distinct().ToList();
        var resourceEntities = requestedResourceIds.Count == 0
            ? []
            : await repositoryFactory.ResourceRepository.GetByIdsAsync(requestedResourceIds, false, cancellationToken);
        if (resourceEntities.Count != requestedResourceIds.Count)
        {
            throw new ResourceNotFound();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var recurringBookingEntity = mapper.MergeTo(
            recurringBooking,
            existingRecurringBooking,
            customerEntities,
            organizations,
            teams,
            resourceEntities,
            existingRecurringBooking.CreatedByCustomer,
            lastModifiedByCustomer,
            null,
            null);

        recurringBookingEntity = repositoryFactory.RecurringBookingRepository.Update(recurringBookingEntity);
        recurringBooking = mapper.MapTo(recurringBookingEntity);

        temporalOutboxService.SignalWorkflowBookPrivateRecurringResourcesUpdated(recurringBooking.Id, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return recurringBooking;
    }

    public async Task<RecurringBooking> DeleteAsync(
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken)
    {
        if (existingRecurringBooking.Channel.ToBookingChannel() != BookingChannel.Private)
        {
            throw new RecurringBookingIsNotPrivate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingRecurringBooking.DeletedByCustomer = deletedByCustomer;
        existingRecurringBooking = repositoryFactory.RecurringBookingRepository.Update(existingRecurringBooking);
        var deletedRecurringBooking = mapper.MapTo(repositoryFactory.RecurringBookingRepository.Remove(existingRecurringBooking));

        temporalOutboxService.SignalWorkflowBookPrivateRecurringResourcesDeleted(existingRecurringBooking.Id, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedRecurringBooking;
    }
}
