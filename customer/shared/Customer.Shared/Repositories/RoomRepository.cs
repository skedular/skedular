using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface IRoomRepository : IRepository<Room>
{
    Task<Room> UpsertNakedAsync(string id, Location location, CancellationToken cancellationToken);
    Task<Room?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Room Add(Room room);
    Room Update(Room room);
    void RemoveRange(ICollection<Room> rooms);
    Task<ICollection<Room>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken);
}

public class RoomRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Room>(dbContext, timeProvider), IRoomRepository
{
    public async Task<Room> UpsertNakedAsync(string id, Location location, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Location>(id, location, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

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

    public async Task<Room?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Room
            .Include(query => query.Location)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Room>> GetByLocationIdAsync(string locationId, CancellationToken cancellationToken) =>
        await DbContext.Room
            .Where(query => query.Location.Id == locationId)
            .ToListAsync(cancellationToken);
}
