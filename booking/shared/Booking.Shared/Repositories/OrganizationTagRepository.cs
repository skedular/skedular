using Booking.Shared.Database;
using Booking.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Booking.Shared.Repositories;

public interface IOrganizationTagRepository : IRepository<OrganizationTag>
{
    Task<OrganizationTag> UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken);
    Task<OrganizationTag?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationTag Add(OrganizationTag organizationTag);
    OrganizationTag Update(OrganizationTag organizationTag);
    void RemoveRange(ICollection<OrganizationTag> organizationTags);
}

public class OrganizationTagRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, OrganizationTag>(dbContext), IOrganizationTagRepository
{
    public async Task<OrganizationTag>
        UpsertNakedAsync(string id, Organization organization, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.OrganizationTag
            .Add(new OrganizationTag { Id = id, CreatedAt = now, Organization = organization }).Entity;
    }

    public OrganizationTag Add(OrganizationTag organizationTag)
    {
        var now = timeProvider.GetUtcNow();
        organizationTag.CreatedAt = now;
        return DbContext.OrganizationTag.Add(organizationTag).Entity;
    }

    public void RemoveRange(ICollection<OrganizationTag> organizationTags)
    {
        var now = timeProvider.GetUtcNow();
        organizationTags.ForEach(organizationTag => organizationTag.DeletedAt = now);
        DbContext.OrganizationTag.UpdateRange(organizationTags);
    }

    public OrganizationTag Update(OrganizationTag organizationTag)
    {
        var now = timeProvider.GetUtcNow();
        organizationTag.ModifiedAt = now;
        return DbContext.OrganizationTag.Update(organizationTag).Entity;
    }

    public async Task<OrganizationTag?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationTag
            .Where(query => query.Id == id)
            .Include(query => query.Organization)
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);
}
