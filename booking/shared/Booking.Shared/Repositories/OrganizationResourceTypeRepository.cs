using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IOrganizationResourceTypeRepository : IRepository<OrganizationResourceType>
{
    Task<OrganizationResourceType> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken);
    Task<OrganizationResourceType?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationResourceType Add(OrganizationResourceType organizationResourceType);
    OrganizationResourceType Update(OrganizationResourceType organizationResourceType);
    void RemoveRange(ICollection<OrganizationResourceType> organizationResourceTypes);
}

public class OrganizationResourceTypeRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, OrganizationResourceType>(dbContext, timeProvider), IOrganizationResourceTypeRepository
{
    public async Task<OrganizationResourceType> UpsertNakedAsync(string id, Organization? organization, CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization>(id, organization, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public OrganizationResourceType Add(OrganizationResourceType organizationResourceType)
    {
        var now = TimeProvider.GetUtcNow();
        organizationResourceType.CreatedAt = now;
        return DbContext.OrganizationResourceType.Add(organizationResourceType).Entity;
    }

    public void RemoveRange(ICollection<OrganizationResourceType> organizationResourceTypes)
    {
        var now = TimeProvider.GetUtcNow();
        organizationResourceTypes.ForEach(organizationResourceType => organizationResourceType.DeletedAt = now);
        DbContext.OrganizationResourceType.UpdateRange(organizationResourceTypes);
    }

    public OrganizationResourceType Update(OrganizationResourceType organizationResourceType)
    {
        var now = TimeProvider.GetUtcNow();
        organizationResourceType.ModifiedAt = now;
        return DbContext.OrganizationResourceType.Update(organizationResourceType).Entity;
    }

    public async Task<OrganizationResourceType?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationResourceType
            .Include(query => query.Organization)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
}
