using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Location.Shared.Database;
using Location.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using LocationRestrictedInformation = Location.Shared.Database.Entities.LocationRestrictedInformation;

namespace Location.Shared.Repositories;

public interface ILocationRestrictedInformationRepository : IRepository<LocationRestrictedInformation>
{
    Task<IReadOnlyList<LocationRestrictedInformation>> GetActiveByLocationIdUntrackedAsync(string locationId, CancellationToken cancellationToken);
    Task<LocationRestrictedInformation?> GetByIdAsync(string id, CancellationToken cancellationToken);
    void Add(LocationRestrictedInformation restrictedInformation);
    void Update(LocationRestrictedInformation restrictedInformation);
    void Remove(LocationRestrictedInformation restrictedInformation);
}

public static class LocationRestrictedInformationExtensions
{
    extension(IQueryable<LocationRestrictedInformation> originalQuery)
    {
        public IIncludableQueryable<LocationRestrictedInformation, Organization> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.Location)
            .ThenInclude(query => query.Organization);
    }
}

public class LocationRestrictedInformationRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, LocationRestrictedInformation>(dbContext, timeProvider), ILocationRestrictedInformationRepository
{
    public async Task<IReadOnlyList<LocationRestrictedInformation>> GetActiveByLocationIdUntrackedAsync(
        string locationId,
        CancellationToken cancellationToken) =>
        await DbContext.LocationRestrictedInformation
            .Where(query => query.LocationId == locationId && query.Active)
            .OrderBy(query => query.SortOrder)
            .ThenBy(query => query.Title)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken);

    public async Task<LocationRestrictedInformation?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.LocationRestrictedInformation
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public void Add(LocationRestrictedInformation restrictedInformation)
    {
        var now = TimeProvider.GetUtcNow();
        restrictedInformation.CreatedAt = now;
        DbContext.LocationRestrictedInformation.Add(restrictedInformation);
    }

    public void Update(LocationRestrictedInformation restrictedInformation)
    {
        var now = TimeProvider.GetUtcNow();
        restrictedInformation.ModifiedAt = now;
        DbContext.LocationRestrictedInformation.Update(restrictedInformation);
    }

    public void Remove(LocationRestrictedInformation restrictedInformation) => DbContext.LocationRestrictedInformation.Remove(restrictedInformation);
}
