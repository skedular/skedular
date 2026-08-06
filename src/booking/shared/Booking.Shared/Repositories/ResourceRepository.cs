using System.Data;
using System.Linq.Expressions;
using Api.Shared.Services.Models;
using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResourceAvailabilityClassification = Booking.Shared.Models.ResourceAvailabilityClassification;
using ResourceAvailabilityOrder = Booking.Shared.Models.ResourceAvailabilityOrder;
using ResourceAvailabilityOrderByField = Booking.Shared.Models.ResourceAvailabilityOrderByField;
using ResourceAvailabilityResourceRow = Booking.Shared.Models.ResourceAvailabilityResourceRow;
using ResourceBookingWindowRow = Booking.Shared.Models.ResourceBookingWindowRow;
using ResourceSlotClaimResult = Booking.Shared.Models.ResourceSlotClaimResult;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Shared.Repositories;

public interface IResourceRepository : IRepository<Resource>
{
    Task<Resource> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken);
    Task<Resource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Task<IReadOnlyList<Resource>> GetByIdsAsync(IReadOnlyList<string> ids, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Resource Add(Resource resource);
    Resource Update(Resource resource);
    void RemoveRange(IEnumerable<Resource> resources);
    Task<IReadOnlyList<Resource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ResourceAvailabilityResourceRow>> GetForAvailabilityDayViewAsync(
        string organizationCustomDomain,
        IReadOnlyList<string> locationIds,
        string? zoneId,
        string? resourceType,
        IReadOnlyList<ResourceAvailabilityClassification> statuses,
        DateTimeOffset dayStart,
        DateTimeOffset dayEnd,
        IReadOnlyList<ResourceAvailabilityOrder> orderBy,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Resource>> GetAvailableResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken);

    Task<int> GetAvailableResourcesCountAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken);

    Task<ResourceSlotClaimResult> TryClaimCompleteSlotSetAsync(
        BookingEntity booking,
        IReadOnlyCollection<string> resourceIds,
        CancellationToken cancellationToken);

    Task ReleaseClaimAsync(string bookingId, CancellationToken cancellationToken);
}

public class ResourceRepository(BookingDbContext dbContext, TimeProvider timeProvider, ILogger<ResourceRepository> logger)
    : RepositoryBase<BookingDbContext, Resource>(dbContext, timeProvider), IResourceRepository
{
    private const int ClaimRetryLimit = 3;

    public async Task<ResourceSlotClaimResult> TryClaimCompleteSlotSetAsync(
        BookingEntity booking,
        IReadOnlyCollection<string> resourceIds,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(booking.Id);
        if (resourceIds.Count == 0)
        {
            return ResourceSlotClaimResult.Success();
        }

        var distinctResourceIds = resourceIds.Distinct().ToList();
        if (DbContext.Database.CurrentTransaction is not null)
        {
            return await TryClaimWithinCurrentTransactionAsync(booking, distinctResourceIds, cancellationToken);
        }

        var claimBooking = booking;
        for (var attempt = 1; attempt <= ClaimRetryLimit; attempt++)
        {
            logger.LogInformation("Attempting atomic resource-slot claim. BookingId={BookingId}, ResourceCount={ResourceCount}, Attempt={Attempt}",
                booking.Id, distinctResourceIds.Count, attempt);
            await using var transaction = await DbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            try
            {
                var slotStates = await DbContext.ResourceBookingSlot
                    .Where(item => distinctResourceIds.Contains(item.ResourceId) && item.Start >= claimBooking.From &&
                                   item.Start < claimBooking.Until)
                    .Select(item => new
                    {
                        item.Id,
                        item.ResourceId,
                        item.Available,
                        IsBooked = item.Bookings.Any(existing => existing.DeletedByCustomer == null),
                    })
                    .ToListAsync(cancellationToken);

                var unavailableResourceIds = distinctResourceIds
                    .Where(resourceId =>
                    {
                        var claimedSlots = slotStates.Where(slot => slot.ResourceId == resourceId).ToList();
                        return claimedSlots.Count == 0 || claimedSlots.Any(slot => !slot.Available || slot.IsBooked);
                    })
                    .ToList();
                if (unavailableResourceIds.Count != 0)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    logger.LogInformation(
                        "Atomic resource-slot claim found unavailable resources. BookingId={BookingId}, UnavailableResourceCount={UnavailableResourceCount}, Attempt={Attempt}",
                        booking.Id, unavailableResourceIds.Count, attempt);
                    return ResourceSlotClaimResult.Conflict(unavailableResourceIds);
                }

                var slots = await DbContext.ResourceBookingSlot
                    .Where(item => slotStates.Select(state => state.Id).Contains(item.Id))
                    .ToListAsync(cancellationToken);
                foreach (var slot in slots)
                {
                    if (slot.Bookings.All(existing => existing.Id != claimBooking.Id))
                    {
                        slot.Bookings.Add(claimBooking);
                    }

                    foreach (var customer in claimBooking.InvolvedCustomers)
                    {
                        if (slot.Customers.All(existing => existing.Id != customer.Id))
                        {
                            slot.Customers.Add(customer);
                        }
                    }

                    slot.ModifiedAt = TimeProvider.GetUtcNow();
                }

                await DbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                logger.LogInformation("Atomic resource-slot claim succeeded. BookingId={BookingId}, ResourceCount={ResourceCount}, Attempt={Attempt}",
                    booking.Id, distinctResourceIds.Count, attempt);
                return ResourceSlotClaimResult.Success();
            }
            catch (DbUpdateException exception) when (IsSerializableConflict(exception) && attempt < ClaimRetryLimit)
            {
                await transaction.RollbackAsync(cancellationToken);
                DbContext.ChangeTracker.Clear();
                claimBooking = await DbContext.Booking.FirstAsync(item => item.Id == booking.Id, cancellationToken);
                logger.LogWarning("Retrying atomic resource-slot claim after a serialization conflict. BookingId={BookingId}, Attempt={Attempt}",
                    booking.Id, attempt);
            }
            catch (DbUpdateException exception) when (IsSerializableConflict(exception))
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogWarning("Atomic resource-slot claim exhausted serialization retries. BookingId={BookingId}, ResourceCount={ResourceCount}",
                    booking.Id, distinctResourceIds.Count);
                return ResourceSlotClaimResult.Conflict(distinctResourceIds, true);
            }
            catch (InvalidOperationException exception) when (IsSerializableConflict(exception))
            {
                await transaction.RollbackAsync(cancellationToken);
                logger.LogWarning("Atomic resource-slot claim exhausted serialization retries. BookingId={BookingId}, ResourceCount={ResourceCount}",
                    booking.Id, distinctResourceIds.Count);
                return ResourceSlotClaimResult.Conflict(distinctResourceIds, true);
            }
        }

        return ResourceSlotClaimResult.Conflict(distinctResourceIds, true);
    }

    public async Task ReleaseClaimAsync(string bookingId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingId);
        for (var attempt = 1; attempt <= ClaimRetryLimit; attempt++)
        {
            logger.LogInformation("Releasing atomic resource-slot claim. BookingId={BookingId}, Attempt={Attempt}", bookingId, attempt);
            await using var transaction = await DbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
            try
            {
                var slots = await DbContext.ResourceBookingSlot
                    .Where(slot => slot.Bookings.Any(booking => booking.Id == bookingId))
                    .Include(slot => slot.Bookings.Where(booking => booking.Id == bookingId))
                    .ToListAsync(cancellationToken);
                foreach (var slot in slots)
                {
                    slot.Bookings.Clear();
                    slot.ModifiedAt = TimeProvider.GetUtcNow();
                }

                await DbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                logger.LogInformation("Released atomic resource-slot claim. BookingId={BookingId}, Attempt={Attempt}", bookingId, attempt);
                return;
            }
            catch (DbUpdateException exception) when (IsSerializableConflict(exception) && attempt < ClaimRetryLimit)
            {
                await transaction.RollbackAsync(cancellationToken);
                DbContext.ChangeTracker.Clear();
                logger.LogWarning("Retrying atomic resource-slot release after a serialization conflict. BookingId={BookingId}, Attempt={Attempt}",
                    bookingId, attempt);
            }
        }
    }

    public async Task<Resource> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Location>(id, location, cancellationToken);

        return (await GetByIdAsync(id, false, cancellationToken))!;
    }

    public async Task<IReadOnlyList<Resource>> GetByIdsAsync(
        IReadOnlyList<string> ids,
        bool includeAllRelatedEntities,
        CancellationToken cancellationToken) =>
        includeAllRelatedEntities
            ? await DbContext.Resource
                .Where(query => ids.Contains(query.Id))
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .ToListAsync(cancellationToken)
            : await DbContext.Resource
                .Where(query => ids.Contains(query.Id))
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .ToListAsync(cancellationToken);

    public Resource Add(Resource resource)
    {
        var now = TimeProvider.GetUtcNow();
        resource.CreatedAt = now;
        return DbContext.Resource.Add(resource).Entity;
    }

    public void RemoveRange(IEnumerable<Resource> resources)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.Resource.UpdateRange(resources.Select(item =>
        {
            item.DeletedAt = now;
            return item;
        }));
    }

    public Resource Update(Resource resource)
    {
        var now = TimeProvider.GetUtcNow();
        resource.ModifiedAt = now;
        return DbContext.Resource.Update(resource).Entity;
    }

    public async Task<Resource?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken) =>
        includeAllRelatedEntities
            ? await DbContext.Resource
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken)
            : await DbContext.Resource
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Resource>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.Resource
            .Where(query => query.Location != null && query.Location.Id == locationId)
            .Include(query => query.OrganizationTags)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ResourceAvailabilityResourceRow>> GetForAvailabilityDayViewAsync(
        string organizationCustomDomain,
        IReadOnlyList<string> locationIds,
        string? zoneId,
        string? resourceType,
        IReadOnlyList<ResourceAvailabilityClassification> statuses,
        DateTimeOffset dayStart,
        DateTimeOffset dayEnd,
        IReadOnlyList<ResourceAvailabilityOrder> orderBy,
        CancellationToken cancellationToken)
    {
        // Organization is a mandatory tenancy boundary — it is always applied as a hard
        // WHERE clause, so a query can never accidentally return resources from another
        // organization, even if the caller passes an empty string (which matches nothing).
        var query = DbContext.Resource
            .AsNoTracking()
            .Where(item => !item.DeletedAt.HasValue &&
                           item.Location != null &&
                           item.Location.Organization != null &&
                           item.Location.Organization.CustomDomain == organizationCustomDomain)
            .AsQueryable();

        if (locationIds.Count > 0)
        {
            query = query.Where(item => item.Location != null && locationIds.Contains(item.Location.Id));
        }

        if (zoneId is not null)
        {
            query = query
                .Where(item => item.OrganizationTags.Any(organizationTag =>
                    !organizationTag.DeletedAt.HasValue &&
                    organizationTag.Type == OrganizationTagTypeConstants.Zone &&
                    organizationTag.Id == zoneId));
        }

        if (resourceType is not null)
        {
            query = query
                .Where(item => item.OrganizationTags.Any(organizationTag =>
                    !organizationTag.DeletedAt.HasValue && organizationTag.Type == resourceType));
        }

        query = ApplyAvailabilityStatusPrefilter(query, statuses, dayStart, dayEnd);

        return await ApplyOrderBy(query, orderBy)
            .Select(item => new ResourceAvailabilityResourceRow
            {
                Id = item.Id,
                Name = item.Name ?? string.Empty,
                Inactive = item.Inactive,
                LocationId = item.Location != null ? item.Location.Id : string.Empty,
                LocationName = item.Location != null ? item.Location.Name ?? string.Empty : string.Empty,
                OrganizationType = item.Location != null && item.Location.Organization != null
                    ? item.Location.Organization.Type
                    : OrganizationTypeConstants.Private,
                OpeningHours = item.Location != null ? item.Location.OpeningHours : null,
                ZoneId = item.OrganizationTags
                    .Where(organizationTag =>
                        !organizationTag.DeletedAt.HasValue &&
                        organizationTag.Type == OrganizationTagTypeConstants.Zone)
                    .Select(organizationTag => organizationTag.Id)
                    .FirstOrDefault(),
                ZoneName = item.OrganizationTags
                    .Where(organizationTag =>
                        !organizationTag.DeletedAt.HasValue &&
                        organizationTag.Type == OrganizationTagTypeConstants.Zone)
                    .Select(organizationTag => organizationTag.Name)
                    .FirstOrDefault(),
                ResourceType = item.OrganizationTags
                    .Where(organizationTag =>
                        !organizationTag.DeletedAt.HasValue &&
                        organizationTag.Type != null &&
                        OrganizationTagTypeConstants.ResourceTagTypes.Contains(organizationTag.Type))
                    .Select(organizationTag => organizationTag.Type)
                    .FirstOrDefault() ?? string.Empty,
                // Booking windows come from the direct Booking many-to-many (BookingResource
                // join table) rather than the ResourceBookingSlot → Booking path.  Filtering
                // by Booking.From/Until overlap covers the whole day in one pass; the
                // BookingResource.InvolvedResourcesId FK index keeps the join efficient.
                BookingWindows = item.InvolvedBookings
                    .Where(b => b.DeletedByCustomer == null &&
                                b.From < dayEnd &&
                                b.Until > dayStart)
                    .Select(b => new ResourceBookingWindowRow
                    {
                        ResourceId = item.Id,
                        BookingId = b.Id,
                        From = b.From,
                        Until = b.Until,
                        IsRecurring = b.RecurringBooking != null,
                        CustomerId = b.CreatedByCustomer != null ? b.CreatedByCustomer.Id : null,
                        CustomerName = b.CreatedByCustomer != null ? b.CreatedByCustomer.Name : null,
                        CustomerGivenName = b.CreatedByCustomer != null ? b.CreatedByCustomer.GivenName : null,
                        CustomerFamilyName = b.CreatedByCustomer != null ? b.CreatedByCustomer.FamilyName : null,
                        Notes = b.Notes,
                    })
                    .OrderBy(b => b.From)
                    .ToList(),
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Resource>> GetAvailableResourcesAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken)
    {
        var availableResourceIds =
            await GetAvailableResourceIdsAsync(organizationId, locationId, from, until, resourceIds, tagIds, tagTypes, cancellationToken);

        var resources = await DbContext.Resource
            .Where(query => availableResourceIds.Contains(query.Id))
            .Include(query => query.ResourceBookingSlots.Where(slot => slot.Start >= from && slot.Start < until).OrderBy(slot => slot.Start))
            .ThenInclude(query => query.Bookings)
            .Include(query => query.ResourceBookingSlots.Where(slot => slot.Start >= from && slot.Start < until).OrderBy(slot => slot.Start))
            .ThenInclude(query => query.Customers)
            .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.Location)
            .OrderBy(query => query.Id)
            .ToListAsync(cancellationToken);

        return resources;
    }

    public async Task<int> GetAvailableResourcesCountAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken) =>
        (await GetAvailableResourceIdsAsync(organizationId, locationId, from, until, resourceIds, tagIds, tagTypes, cancellationToken)).Count;

    // Translates ResourceAvailabilityOrder clauses into EF IQueryable ORDER BY expressions so the
    // database engine handles sorting rather than the application layer.
    // Status and FloorName have no DB columns in v1 and fall back to ResourceName.
    private static IQueryable<Resource> ApplyOrderBy(IQueryable<Resource> query, IReadOnlyList<ResourceAvailabilityOrder> orderBy)
    {
        if (orderBy.Count == 0)
        {
            return query.OrderBy(item => item.Name);
        }

        var clauses = orderBy.Select(item => (Key: GetKeySelector(item.Field), item.Direction)).ToList();
        var ordered = clauses[0].Direction == OrderDirection.Ascending ? query.OrderBy(clauses[0].Key) : query.OrderByDescending(clauses[0].Key);

        return clauses
            .Skip(1)
            .Aggregate(ordered, (q, c) => c.Direction == OrderDirection.Ascending ? q.ThenBy(c.Key) : q.ThenByDescending(c.Key));
    }

    // Maps each ResourceAvailabilityOrderByField to its SQL-translatable key selector.
    // FloorName and Status are not DB columns in v1 — both fall back to ResourceName.
    private static Expression<Func<Resource, object?>> GetKeySelector(ResourceAvailabilityOrderByField field) =>
        field switch
        {
            ResourceAvailabilityOrderByField.ResourceName => item => item.Name,
            ResourceAvailabilityOrderByField.LocationName => item => item.Location!.Name,
            ResourceAvailabilityOrderByField.ResourceType =>
                item => item.OrganizationTags
                    .Where(organizationTag =>
                        organizationTag.Type != null && OrganizationTagTypeConstants.ResourceTagTypes.Contains(organizationTag.Type))
                    .Select(organizationTag => organizationTag.Type)
                    .FirstOrDefault(),
            ResourceAvailabilityOrderByField.ZoneName =>
                item => item.OrganizationTags
                    .Where(organizationTag => organizationTag.Type == OrganizationTagTypeConstants.Zone)
                    .Select(organizationTag => organizationTag.Name)
                    .FirstOrDefault(),
            _ => throw new ArgumentOutOfRangeException(nameof(field), field,
                $"Unexpected value for {nameof(field)}: {field}. Update enum mapping or caller input."),
        };

    private static IQueryable<Resource> ApplyAvailabilityStatusPrefilter(
        IQueryable<Resource> query,
        IReadOnlyList<ResourceAvailabilityClassification> statuses,
        DateTimeOffset dayStart,
        DateTimeOffset dayEnd)
    {
        if (statuses.Count == 0)
        {
            return query;
        }

        var statusSet = statuses.ToHashSet();
        if (statusSet.SetEquals([ResourceAvailabilityClassification.Blocked]))
        {
            return query.Where(item => item.Inactive);
        }

        if (!statusSet.Contains(ResourceAvailabilityClassification.Blocked))
        {
            query = query.Where(item => !item.Inactive);
        }

        var bookedStatuses = new[] { ResourceAvailabilityClassification.PartiallyBooked, ResourceAvailabilityClassification.FullyBooked };

        // Pre-filter uses InvolvedBookings (the direct Booking M2M) instead of the old
        // ResourceBookingSlot path.  Interval-overlap semantics (From < dayEnd && Until > dayStart)
        // match the projection filter, so the pre-filter and the final in-memory classification
        // stay consistent with each other.
        if (statusSet.All(status => bookedStatuses.Contains(status)))
        {
            return query.Where(item => item.InvolvedBookings.Any(b =>
                b.DeletedByCustomer == null &&
                b.From < dayEnd &&
                b.Until > dayStart));
        }

        if (statusSet.SetEquals([ResourceAvailabilityClassification.Available]))
        {
            return query.Where(item => !item.InvolvedBookings.Any(b =>
                b.DeletedByCustomer == null &&
                b.From < dayEnd &&
                b.Until > dayStart));
        }

        return query;
    }

    private async Task<IReadOnlyList<string>> GetAvailableResourceIdsAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset from,
        DateTimeOffset until,
        IReadOnlyList<string> resourceIds,
        IReadOnlyList<string> tagIds,
        IReadOnlyList<string> tagTypes,
        CancellationToken cancellationToken)
    {
        var slots = await DbContext.ResourceBookingSlot
            .Where(query => !query.Resource.DeletedAt.HasValue &&
                            !query.Resource.Inactive &&
                            (resourceIds.Count == 0 || resourceIds.Contains(query.Resource.Id)) &&
                            query.Start >= from && query.Start < until &&
                            (string.IsNullOrWhiteSpace(organizationId) || (query.Resource.Location != null &&
                                                                           !query.Resource.Location.DeletedAt.HasValue &&
                                                                           query.Resource.Location.Organization != null &&
                                                                           !query.Resource.Location.Organization.DeletedAt.HasValue &&
                                                                           query.Resource.Location.Organization.Id == organizationId)) &&
                            (string.IsNullOrWhiteSpace(locationId) ||
                             (query.Resource.Location != null &&
                              !query.Resource.Location.DeletedAt.HasValue &&
                              query.Resource.Location.Organization != null &&
                              !query.Resource.Location.Organization.DeletedAt.HasValue &&
                              query.Resource.Location.Id == locationId)) &&
                            (tagIds.Count == 0 || query.Resource.OrganizationTags.Any(tag => !tag.DeletedAt.HasValue && tagIds.Contains(tag.Id))) &&
                            (tagTypes.Count == 0 || query.Resource.OrganizationTags.Any(tag =>
                                !tag.DeletedAt.HasValue && !string.IsNullOrWhiteSpace(tag.Type) && tagTypes.Contains(tag.Type))))
            .Include(query => query.Bookings)
            .Include(query => query.Resource)
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);

        return slots
            .GroupBy(slot => slot.Resource.Id)
            .Select(group => new
            {
                group.First().Resource,
                Slots = group.ToList(),
            })
            .Where(grouped => grouped.Slots.All(slot => slot is { Available: true, Bookings.Count: 0 }))
            .GroupBy(slot => slot.Resource.Id)
            .Select(item => item.Key)
            .ToList();
    }

    private static bool IsSerializableConflict(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current.Message.Contains("40001", StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private async Task<ResourceSlotClaimResult> TryClaimWithinCurrentTransactionAsync(
        BookingEntity booking,
        IReadOnlyCollection<string> resourceIds,
        CancellationToken cancellationToken)
    {
        var slotStates = await DbContext.ResourceBookingSlot
            .Where(item => resourceIds.Contains(item.ResourceId) && item.Start >= booking.From && item.Start < booking.Until)
            .Select(item => new
            {
                item.Id,
                item.ResourceId,
                item.Available,
                IsBooked = item.Bookings.Any(existing => existing.DeletedByCustomer == null),
            })
            .ToListAsync(cancellationToken);
        var unavailableResourceIds = resourceIds
            .Where(resourceId =>
            {
                var claimedSlots = slotStates.Where(slot => slot.ResourceId == resourceId).ToList();
                return claimedSlots.Count == 0 || claimedSlots.Any(slot => !slot.Available || slot.IsBooked);
            })
            .ToList();
        if (unavailableResourceIds.Count != 0)
        {
            logger.LogInformation(
                "Atomic resource-slot claim found unavailable resources in the caller transaction. BookingId={BookingId}, UnavailableResourceCount={UnavailableResourceCount}",
                booking.Id, unavailableResourceIds.Count);
            return ResourceSlotClaimResult.Conflict(unavailableResourceIds);
        }

        var slots = await DbContext.ResourceBookingSlot
            .Where(item => slotStates.Select(state => state.Id).Contains(item.Id))
            .ToListAsync(cancellationToken);
        foreach (var slot in slots)
        {
            if (slot.Bookings.All(existing => existing.Id != booking.Id))
            {
                slot.Bookings.Add(booking);
            }

            foreach (var customer in booking.InvolvedCustomers)
            {
                if (slot.Customers.All(existing => existing.Id != customer.Id))
                {
                    slot.Customers.Add(customer);
                }
            }

            slot.ModifiedAt = TimeProvider.GetUtcNow();
        }

        logger.LogInformation("Atomic resource-slot claim attached to the caller transaction. BookingId={BookingId}, ResourceCount={ResourceCount}",
            booking.Id, resourceIds.Count);
        return ResourceSlotClaimResult.Success();
    }
}
