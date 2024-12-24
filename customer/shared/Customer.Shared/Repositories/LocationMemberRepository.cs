using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface ILocationMemberRepository : IRepository<LocationMember>
{
    LocationMember Add(LocationMember locationMember);
    LocationMember Update(LocationMember locationMember);
    void RemoveRange(ICollection<LocationMember> locationMembers);

    Task<ICollection<LocationMember>> GetByLocationIdAsync(
        string locationId,
        CancellationToken cancellationToken);
}

public class LocationMemberRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, LocationMember>(dbContext, timeProvider), ILocationMemberRepository
{
    public LocationMember Add(LocationMember locationMember)
    {
        var now = TimeProvider.GetUtcNow();
        locationMember.CreatedAt = now;
        return DbContext.LocationMember.Add(locationMember).Entity;
    }

    public void RemoveRange(ICollection<LocationMember> locationMembers)
    {
        var now = TimeProvider.GetUtcNow();
        locationMembers.ForEach(locationMember => locationMember.DeletedAt = now);
        DbContext.LocationMember.UpdateRange(locationMembers);
    }

    public LocationMember Update(LocationMember locationMember)
    {
        var now = TimeProvider.GetUtcNow();
        locationMember.ModifiedAt = now;
        return DbContext.LocationMember.Update(locationMember).Entity;
    }

    public async Task<ICollection<LocationMember>> GetByLocationIdAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.LocationMember
            .Where(query => query.Location.Id == organizationId)
            .Include(query => query.Customer)
            .ToListAsync(cancellationToken);
}
