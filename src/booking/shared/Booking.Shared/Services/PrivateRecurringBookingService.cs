using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
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
    IEntityMapper entityMapper,
    ITemporalOutboxService temporalOutboxService,
    ISpacesBookingQuotaService spacesBookingQuotaService) : IPrivateRecurringBookingService
{
    public async Task<RecurringBooking> AddAsync(
        RecurringBooking recurringBooking,
        Customer customer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken)
    {
        await EnsureSpacesAccessAsync(organizations, cancellationToken);

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

        var recurringBookingEntity = entityMapper.MapTo(
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
        recurringBooking = entityMapper.MapTo(recurringBookingEntity);

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

        await EnsureSpacesAccessAsync(organizations, cancellationToken);

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

        var recurringBookingEntity = entityMapper.MergeTo(
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
        recurringBooking = entityMapper.MapTo(recurringBookingEntity);

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
        var deletedRecurringBooking = entityMapper.MapTo(repositoryFactory.RecurringBookingRepository.Remove(existingRecurringBooking));

        temporalOutboxService.SignalWorkflowBookPrivateRecurringResourcesDeleted(existingRecurringBooking.Id, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedRecurringBooking;
    }

    private async Task EnsureSpacesAccessAsync(
        IReadOnlyList<Organization> organizations,
        CancellationToken cancellationToken)
    {
        foreach (var organization in organizations
                     .Where(item => item.Type == OrganizationTypeConstants.Marketplace)
                     .DistinctBy(item => item.Id))
        {
            var decision = await spacesBookingQuotaService.EvaluateAccessAsync(
                organization.Id,
                SpacesAccessAction.CreateOrModify,
                cancellationToken);
            if (!decision.Allowed)
            {
                throw new SpacesAccessDenied(decision);
            }
        }
    }
}
