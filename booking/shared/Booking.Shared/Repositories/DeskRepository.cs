using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IDeskRepository : IRepository<Desk>
{
    Task<Desk> UpsertNakedAsync(string id, Location? location, CancellationToken cancellationToken);
    Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Desk Add(Desk desk);
    Desk Update(Desk desk);
    void RemoveRange(ICollection<Desk> desks);
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
        var now = TimeProvider.GetUtcNow();
        desks.ForEach(desk => desk.DeletedAt = now);
        DbContext.Desk.UpdateRange(desks);
    }

    public Desk Update(Desk desk)
    {
        var now = TimeProvider.GetUtcNow();
        desk.ModifiedAt = now;
        return DbContext.Desk.Update(desk).Entity;
    }

    public async Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Desk
            .Where(query => query.Id == id)
            .Include(query => query.Location)
            .Include(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);
}
