using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface IDeskRepository : IRepository<Desk>
{
    Task<Desk> UpsertNakedAsync(string id, Location location, CancellationToken cancellationToken);
    Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Desk Add(Desk desk);
    Desk Update(Desk desk);
    void RemoveRange(ICollection<Desk> desks);
}

public class DeskRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, Desk>(dbContext, timeProvider), IDeskRepository
{
    public async Task<Desk> UpsertNakedAsync(string id, Location location, CancellationToken cancellationToken)
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

    public void RemoveRange(ICollection<Desk> desks) => DbContext.Desk.RemoveRange(desks);

    public Desk Update(Desk desk)
    {
        var now = TimeProvider.GetUtcNow();
        desk.ModifiedAt = now;
        return DbContext.Desk.Update(desk).Entity;
    }

    public async Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Desk
            .Include(query => query.Location)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
}
