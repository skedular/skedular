using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Room = Location.Shared.Database.Entities.Room;
using OrganizationTag = Location.Shared.Database.Entities.OrganizationTag;

namespace Location.Shared.Repositories;

public interface IRoomRepository : IRepository<Room>
{
    Task<Room?> GetByIdAsync(string id, bool includeBookings, CancellationToken cancellationToken);
    Task<ICollection<Room>> GetByIdsAsync(ICollection<string> ids, bool includeBookings, CancellationToken cancellationToken);
    Room Add(Room room);
    Room Update(Room room);
    void RemoveRange(ICollection<Room> rooms);
    Room Remove(Room room);

    Task<(PaginatedInfo, ICollection<Edge<Room>>, int )> GetPaginatedRoomsAsync(
        PaginationInputParam paginationInputParam,
        RoomSearchCriteria searchCriteria,
        ICollection<RoomOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class RoomExtensions
{
    internal static IIncludableQueryable<Room, IEnumerable<OrganizationTag>> AddDependentObjects(
        this IQueryable<Room> originalQuery,
        bool includeBookings) =>
        includeBookings
            ? originalQuery
                .Include(query => query.Bookings)
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(organizationTag => !organizationTag.DeletedAt.HasValue))
            : originalQuery
                .Include(query => query.Location)
                .Include(query => query.OrganizationTags.Where(organizationTag => !organizationTag.DeletedAt.HasValue));

    internal static IQueryable<Room> AddSearchCriteria(this IQueryable<Room> query, RoomSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Location.Id == searchCriteria.LocationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        if (searchCriteria.ZoneIds.Count != 0)
        {
            query = query.Where(item => searchCriteria.ZoneIds.All(zoneId => item.OrganizationTags.Any(tag => tag.Id == zoneId)));
        }

        if (searchCriteria.CustomTagIds.Count != 0)
        {
            query = query.Where(item => searchCriteria.CustomTagIds.All(customTagId => item.OrganizationTags.Any(tag => tag.Id == customTagId)));
        }

        return query;
    }

    internal static IQueryable<Room> AddSortingOrders(this IQueryable<Room> originalQuery, ICollection<RoomOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            RoomOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                RoomOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class RoomRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Room>(dbContext, timeProvider), IRoomRepository
{
    public async Task<Room?> GetByIdAsync(string id, bool includeBookings, CancellationToken cancellationToken) =>
        await DbContext.Room
            .AddDependentObjects(includeBookings)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Room>> GetByIdsAsync(ICollection<string> ids, bool includeBookings, CancellationToken cancellationToken) =>
        await DbContext.Room
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects(includeBookings)
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

    public Room Remove(Room room)
    {
        var now = TimeProvider.GetUtcNow();
        room.DeletedAt = now;
        return DbContext.Room.Update(room).Entity;
    }

    public Room Update(Room room)
    {
        var now = TimeProvider.GetUtcNow();
        room.ModifiedAt = now;
        return DbContext.Room.Update(room).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Room>>, int)> GetPaginatedRoomsAsync(
        PaginationInputParam paginationInputParam,
        RoomSearchCriteria searchCriteria,
        ICollection<RoomOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Room
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
