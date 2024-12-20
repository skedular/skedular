using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IDeskRepository : IRepository<Desk>
{
    Task<Desk> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken);
    Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Desk Add(Desk desk);
    Desk Update(Desk desk);
    void RemoveRange(ICollection<Desk> desks);

    Task<ICollection<Desk>> GetAvailableDesksAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> deskIdsToInclude,
        ICollection<string> deskTypeIds,
        ICollection<string> zoneIds,
        bool combineDeskTypesZones,
        CancellationToken cancellationToken);
}

public class DeskRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Desk>(dbContext, timeProvider), IDeskRepository
{
    public async Task<Desk> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Location>(id, location, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public Desk Add(Desk desk)
    {
        var now = TimeProvider.GetUtcNow();
        desk.CreatedAt = now;
        return DbContext.Desk.Add(desk).Entity;
    }

    public void RemoveRange(ICollection<Desk> desks)
    {
        DbContext.Desk.RemoveRange(desks);
    }

    public Desk Update(Desk desk)
    {
        var now = TimeProvider.GetUtcNow();
        desk.ModifiedAt = now;
        return DbContext.Desk.Update(desk).Entity;
    }

    public async Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Desk
            .Include(query => query.Location)
            .Include(query => query.OrganizationTags)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Desk>> GetAvailableDesksAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> deskIdsToInclude,
        ICollection<string> deskTypeIds,
        ICollection<string> zoneIds,
        bool combineDeskTypesZones,
        CancellationToken cancellationToken)
    {
        var deskQuery = deskIdsToInclude.Count == 0
            ? DbContext.Desk
                .Where(query => !query.Deactivated &&
                                query.Location != null &&
                                (string.IsNullOrWhiteSpace(organizationId) || (query.Location.Organization != null &&
                                                                               query.Location.Organization.Id ==
                                                                               organizationId)) &&
                                (string.IsNullOrWhiteSpace(locationId) || query.Location.Id == locationId) &&
                                (
                                    string.IsNullOrWhiteSpace(organizationId) ||
                                    !query.Bookings.Any(booking =>
                                        !booking.DeletedAt.HasValue && booking.From >= date &&
                                        booking.To < date.Tomorrow() && booking.Location != null &&
                                        booking.Location.Organization != null &&
                                        booking.Location.Organization.Id == organizationId)
                                ) &&
                                (
                                    string.IsNullOrWhiteSpace(locationId) ||
                                    !query.Bookings.Any(booking =>
                                        !booking.DeletedAt.HasValue && booking.From >= date &&
                                        booking.To < date.Tomorrow() && booking.Location != null &&
                                        booking.Location.Id == locationId)
                                )
                )
            : DbContext.Desk
                .Where(query => (!query.Deactivated &&
                                 query.Location != null &&
                                 (string.IsNullOrWhiteSpace(organizationId) || (query.Location.Organization != null &&
                                     query.Location.Organization.Id ==
                                     organizationId)) &&
                                 (string.IsNullOrWhiteSpace(locationId) || query.Location.Id == locationId) &&
                                 (
                                     string.IsNullOrWhiteSpace(organizationId) ||
                                     !query.Bookings.Any(booking =>
                                         !booking.DeletedAt.HasValue && booking.From >= date &&
                                         booking.To < date.Tomorrow() && booking.Location != null &&
                                         booking.Location.Organization != null &&
                                         booking.Location.Organization.Id == organizationId)
                                 ) &&
                                 (
                                     string.IsNullOrWhiteSpace(locationId) ||
                                     !query.Bookings.Any(booking =>
                                         !booking.DeletedAt.HasValue && booking.From >= date &&
                                         booking.To < date.Tomorrow() && booking.Location != null &&
                                         booking.Location.Id == locationId)
                                 )) || deskIdsToInclude.Contains(query.Id)
                );

        var desks = await deskQuery
            .Include(query => query.Location)
            .Include(query => query.OrganizationTags)
            .OrderBy(query => query.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return desks.Where(item =>
        {
            if (deskIdsToInclude.Count != 0 && deskIdsToInclude.Contains(item.Id))
            {
                return true;
            }

            if (deskTypeIds.Count == 0 && zoneIds.Count == 0)
            {
                return true;
            }

            var organizationTagIds = item.OrganizationTags.Select(tag => tag.Id).ToList();
            var deskTypeMatchResult = deskTypeIds.All(deskTypeId => organizationTagIds.Any(id => id == deskTypeId));
            var zoneMatchResult = zoneIds.All(zoneId => organizationTagIds.Any(id => id == zoneId));

            return combineDeskTypesZones
                ? deskTypeMatchResult && zoneMatchResult
                : deskTypeMatchResult || zoneMatchResult;
        }).ToList();
    }
}
