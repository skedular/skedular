using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface ILocationTagRepository : IRepository<LocationTag>
{
    Task<LocationTag> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken);
    Task<LocationTag?> GetByIdAsync(string id, CancellationToken cancellationToken);
    LocationTag Add(LocationTag locationTag);
    LocationTag Update(LocationTag locationTag);
    void RemoveRange(ICollection<LocationTag> locationTags);
}

public class LocationTagRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, LocationTag>(dbContext), ILocationTagRepository
{
    public async Task<LocationTag>
        UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.LocationTag.Add(new LocationTag { Id = id, CreatedAt = now, Location = location }).Entity;
    }

    public LocationTag Add(LocationTag locationTag)
    {
        var now = timeProvider.GetUtcNow();
        locationTag.CreatedAt = now;
        return DbContext.LocationTag.Add(locationTag).Entity;
    }

    public void RemoveRange(ICollection<LocationTag> locationTags)
    {
        var now = timeProvider.GetUtcNow();
        locationTags.ForEach(locationTag => locationTag.DeletedAt = now);
        DbContext.LocationTag.UpdateRange(locationTags);
    }

    public LocationTag Update(LocationTag locationTag)
    {
        var now = timeProvider.GetUtcNow();
        locationTag.ModifiedAt = now;
        return DbContext.LocationTag.Update(locationTag).Entity;
    }

    public async Task<LocationTag?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.LocationTag
            .Where(query => query.Id == id)
            .Include(query => query.Location)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);
}
