using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IRoomRepository : IRepository<Room>
{
    Task<Room> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken);
    Task<ICollection<Room>> GetAllAsync(bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Task<Room?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken);
    Room Add(Room room);
    Room Update(Room room);
    void RemoveRange(ICollection<Room> rooms);

    Task<ICollection<Room>> GetAvailableRoomsAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> roomIdsToInclude,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool combineCustomTagsZones,
        CancellationToken cancellationToken);

    Task<ICollection<Room>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);
}

public class RoomRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Room>(dbContext, timeProvider), IRoomRepository
{
    public async Task<Room> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Location>(id, location, cancellationToken);

        return (await GetByIdAsync(id, false, cancellationToken))!;
    }

    public async Task<ICollection<Room>> GetAllAsync(bool includeAllRelatedEntities, CancellationToken cancellationToken) =>
        includeAllRelatedEntities
            ? await DbContext.Room
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .ToListAsync(cancellationToken)
            : await DbContext.Room
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .ToListAsync(cancellationToken);

    public Room Add(Room room)
    {
        var now = TimeProvider.GetUtcNow();
        room.CreatedAt = now;
        return DbContext.Room.Add(room).Entity;
    }

    public void RemoveRange(ICollection<Room> rooms)
    {
        var now = TimeProvider.GetUtcNow();
        rooms.ForEach(room => room.DeletedAt = now);
        DbContext.Room.UpdateRange(rooms);
    }

    public Room Update(Room room)
    {
        var now = TimeProvider.GetUtcNow();
        room.ModifiedAt = now;
        return DbContext.Room.Update(room).Entity;
    }

    public async Task<Room?> GetByIdAsync(string id, bool includeAllRelatedEntities, CancellationToken cancellationToken) =>
        includeAllRelatedEntities
            ? await DbContext.Room
                .Include(query => query.PreferredByCustomers)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken)
            : await DbContext.Room
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Room>> GetAvailableRoomsAsync(
        string? organizationId,
        string? locationId,
        DateTimeOffset date,
        ICollection<string> roomIdsToInclude,
        ICollection<string> customTagIds,
        ICollection<string> zoneIds,
        bool combineCustomTagsZones,
        CancellationToken cancellationToken)
    {
        var roomQuery = roomIdsToInclude.Count == 0
            ? DbContext.Room
                .Where(query => !query.DeletedAt.HasValue &&
                                !query.Deactivated &&
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
            : DbContext.Room
                .Where(query => (!query.DeletedAt.HasValue &&
                                 !query.Deactivated &&
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
                                 )) || roomIdsToInclude.Contains(query.Id)
                );

        var rooms = await roomQuery
            .Include(query => query.Location)
            .Include(query => query.OrganizationTags)
            .OrderBy(query => query.Name)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rooms.Where(item =>
        {
            if (roomIdsToInclude.Count != 0 && roomIdsToInclude.Contains(item.Id))
            {
                return true;
            }

            if (customTagIds.Count == 0 && zoneIds.Count == 0)
            {
                return true;
            }

            var organizationTagIds = item.OrganizationTags.Select(tag => tag.Id).ToList();
            var customTagMatchResult = customTagIds.All(customTagId => organizationTagIds.Any(id => id == customTagId));
            var zoneMatchResult = zoneIds.All(zoneId => organizationTagIds.Any(id => id == zoneId));

            return combineCustomTagsZones
                ? customTagMatchResult && zoneMatchResult
                : customTagMatchResult || zoneMatchResult;
        }).ToList();
    }

    public async Task<ICollection<Room>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.Room
            .Where(query => query.Location != null && query.Location.Id == locationId)
            .Include(query => query.OrganizationTags)
            .ToListAsync(cancellationToken);
}
