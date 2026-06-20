using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Microsoft.Extensions.Logging;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Services;

public interface IPrivateBookingService
{
    Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken);

    Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer? lastModifiedByCustomer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        RecurringBooking? recurringBooking,
        bool bookResourceIfNoResourceProvidedOrAvailable,
        CancellationToken cancellationToken);

    Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        bool preserveRecurringSeries,
        CancellationToken cancellationToken);
}

public class PrivateBookingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IBookingOutboxPublisher bookingOutboxPublisher,
    IEntityMapper entityMapper,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    ICachedBookingService cachedBookingService,
    IResourceService resourceService,
    IPrivateBookingPreferenceService privateBookingPreferenceService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    ITemporalOutboxService temporalOutboxService,
    ISpacesBookingQuotaService spacesBookingQuotaService,
    ILogger<PrivateBookingService> logger) : IPrivateBookingService
{
    public async Task<Models.Booking> AddAsync(
        Models.Booking booking,
        Customer customer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        RecurringBooking? recurringBooking,
        CancellationToken cancellationToken)
    {
        ValidateBookingWindowWithinSingleDay(booking);

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

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (booking.InvolvedCustomers.Count == 1 && resourceIds.Count == 0)
        {
            if (resources.Count == 0)
            {
                (organizations, resources) = await privateBookingPreferenceService.PickResourceBasedOnCustomerPreferencesAsync(
                    customerEntities.First(),
                    booking.From,
                    booking.Until,
                    booking.InvolvedOrganizations.Where(item => !string.IsNullOrWhiteSpace(item.Id)).Select(item => item.Id).ToList(),
                    booking.InvolvedOrganizations
                        .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                        .Select(item => item.CustomDomain!)
                        .ToList(),
                    cancellationToken);
            }
        }

        var slots = resources.SelectMany(item => item.ResourceBookingSlots).ToList();
        foreach (var slot in slots)
        {
            foreach (var matchingCustomerEntity in customerEntities)
            {
                slot.Customers.Add(matchingCustomerEntity);
            }
        }

        repositoryFactory.ResourceBookingSlotRepository.UpdateRange(slots);

        var bookingEntity = entityMapper.MapTo(
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
        foreach (var organization in organizations.DistinctBy(item => item.Id).Where(ShouldEnforceSpacesQuota))
        {
            var decision = await spacesBookingQuotaService.TryReserveBookingInstancesAsync(
                organization.Id,
                [booking.From.ToUniversalTime()],
                cancellationToken);

            if (decision.CanCreate)
            {
                continue;
            }

            if (decision.AccessDecision is { Allowed: false } accessDecision)
            {
                throw new SpacesAccessDenied(accessDecision);
            }

            if (decision.ReasonCode == SpacesQuotaReasonCode.MissingOfferingState)
            {
                throw new SpacesOfferingStateMissing();
            }

            throw new SpacesBookingQuotaExceeded(
                decision.ReasonCode,
                decision.CurrentUsage,
                decision.QuotaLimit,
                decision.AttemptedCurrentPeriodCount,
                decision.ExcludedOutOfPeriodCount,
                decision.RemainingQuota,
                decision.UpgradePlans);
        }

        booking = entityMapper.MapTo(bookingEntity);

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

        return booking;
    }

    public async Task<Models.Booking> UpdateAsync(
        Models.Booking booking,
        Database.Entities.Booking existingBooking,
        Customer? lastModifiedByCustomer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        RecurringBooking? recurringBooking,
        bool bookResourceIfNoResourceProvidedOrAvailable,
        CancellationToken cancellationToken)
    {
        ValidateBookingWindowWithinSingleDay(booking);

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

        foreach (var organization in organizations.DistinctBy(item => item.Id).Where(ShouldEnforceSpacesQuota))
        {
            var accessDecision = await spacesBookingQuotaService.EvaluateAccessAsync(
                organization.Id,
                SpacesAccessAction.CreateOrModify,
                cancellationToken);
            if (!accessDecision.Allowed)
            {
                throw new SpacesAccessDenied(accessDecision);
            }
        }

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        /********************************************************************************************************************/
        // TODO: 20250317 : Morteza: For now, remove all existing resources as part of the transaction to make subsequent resource availability check easier to manage.
        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        /********************************************************************************************************************/

        var resourceIds = booking.Resources.Select(item => item.Resource.Id).ToList();
        IReadOnlyList<Resource> resources;

        // For non-customized recurring instances, the scheduler can request a best-effort rebooking.
        // In that mode we first try resources provided on the booking model, and only if none are
        // currently available, we fall back to preference-based auto assignment (the same strategy as AddAsync).
        if (bookResourceIfNoResourceProvidedOrAvailable && existingBooking.HasRecurringInstanceOverrides != true && resourceIds.Count == 0)
        {
            resources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
                null,
                null,
                booking.From,
                booking.Until,
                resourceIds,
                [],
                [],
                cancellationToken);

            // If no requested resource is available, try to auto-pick one by customer preference.
            if (resources.Count == 0 && booking.InvolvedCustomers.Count == 1)
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
                        .Where(item => !string.IsNullOrWhiteSpace(item.CustomDomain))
                        .Select(item => item.CustomDomain!)
                        .ToList(),
                    cancellationToken);
            }
        }
        else
        {
            // Non-recurring or customized instances keep strict behavior:
            // caller-provided resources must all be available.
            resources = await resourceService.GetResourceEntitiesAndValidateAvailabilityAsync(
                booking.From,
                booking.Until,
                resourceIds,
                [],
                cancellationToken);
        }

        var slots = resources.SelectMany(item => item.ResourceBookingSlots).ToList();
        foreach (var slot in slots)
        {
            foreach (var matchingCustomerEntity in customerEntities)
            {
                slot.Customers.Add(matchingCustomerEntity);
            }
        }

        repositoryFactory.ResourceBookingSlotRepository.UpdateRange(slots);

        var bookingEntity = entityMapper.MergeTo(
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
        booking = entityMapper.MapTo(bookingEntity);

        logger.LogInformation("Update path excluded from Spaces quota usage. BookingId: {BookingId}", bookingEntity.Id);

        bookingOutboxPublisher.PublishBookings([booking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        return booking;
    }

    public async Task<Models.Booking> DeleteAsync(
        Database.Entities.Booking existingBooking,
        Customer? deletedByCustomer,
        bool preserveRecurringSeries,
        CancellationToken cancellationToken)
    {
        if (existingBooking.Channel.ToBookingChannel() != BookingChannel.Private)
        {
            throw new BookingIsNotPrivate();
        }

        var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (preserveRecurringSeries && existingBooking.RecurringBooking is not null && !existingBooking.RecurringBooking.IsDeleted())
        {
            var skippedDate = new DateTimeOffset(existingBooking.From.UtcDateTime.Date, TimeSpan.Zero);
            if (existingBooking.RecurringBooking.SkippedDates.All(item => item.UtcDateTime.Date != skippedDate.UtcDateTime.Date))
            {
                existingBooking.RecurringBooking.SkippedDates.Add(skippedDate);
            }

            existingBooking.RecurringBooking.LastModifiedByCustomer = deletedByCustomer;
            repositoryFactory.RecurringBookingRepository.Update(existingBooking.RecurringBooking);
            temporalOutboxService.SignalWorkflowBookPrivateRecurringResourcesUpdated(
                existingBooking.RecurringBooking.Id,
                repositoryFactory.UnitOfWork);
        }

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(existingBooking);

        existingBooking.InvolvedResources.Clear();
        existingBooking.DeletedByCustomer = deletedByCustomer;
        existingBooking = repositoryFactory.BookingRepository.Update(existingBooking);
        var deletedBooking = entityMapper.MapTo(repositoryFactory.BookingRepository.Remove(existingBooking));

        bookingOutboxPublisher.PublishBookings([deletedBooking], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.RemoveByIdAsync(deletedBooking.Id, cancellationToken);

        return deletedBooking;
    }

    private static bool ShouldEnforceSpacesQuota(Organization organization) => organization.Type == OrganizationTypeConstants.Marketplace;

    private static void ValidateBookingWindowWithinSingleDay(Models.Booking booking)
    {
        var from = booking.From.UtcDateTime;
        var until = booking.Until.UtcDateTime;

        if (from.Date != until.Date && (from.Date.AddDays(1) != until.Date || until.TimeOfDay != TimeSpan.Zero))
        {
            throw new BookingMustStartAndEndWithinSameDay();
        }
    }

    private static List<Location> ResourcesToLocations(IReadOnlyList<Resource> resources) =>
        resources
            .Where(item => item.Location is not null)
            .Select(item => item.Location)
            .GroupBy(item => item!.Id)
            .Select(item => item.First())
            .ToList()!;
}
