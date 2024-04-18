using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Notification.Shared.Database;
using Notification.Shared.Database.Entities;

namespace Notification.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Organization Add(Organization organization);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

public class OrganizationRepository(NotificationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<NotificationDbContext, Organization>(dbContext), IOrganizationRepository
{
    public async Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.Organization.Add(new Organization { Id = id, CreatedAt = now }).Entity;
    }

    public async Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => query.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public Organization Add(Organization organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Organization.Add(organization).Entity;
    }

    public Organization Update(Organization organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public Organization Remove(Organization organization)
    {
        var now = timeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }
}
