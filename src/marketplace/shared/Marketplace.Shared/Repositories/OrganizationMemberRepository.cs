using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Marketplace.Shared.Database;
using Marketplace.Shared.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Shared.Repositories;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    OrganizationMember Add(OrganizationMember organizationMember);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(IEnumerable<OrganizationMember> organizationMembers);
    Task<IReadOnlyList<OrganizationMember>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationMemberRepository(MarketplaceDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<MarketplaceDbContext, OrganizationMember>(dbContext, timeProvider), IOrganizationMemberRepository
{
    public OrganizationMember Add(OrganizationMember organizationMember)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMember.CreatedAt = now;
        return DbContext.OrganizationMember.Add(organizationMember).Entity;
    }

    public void RemoveRange(IEnumerable<OrganizationMember> organizationMembers)
    {
        var now = TimeProvider.GetUtcNow();
        DbContext.OrganizationMember.UpdateRange(organizationMembers.Select(item =>
        {
            item.DeletedAt = now;
            return item;
        }));
    }

    public OrganizationMember Update(OrganizationMember organizationMember)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMember.ModifiedAt = now;
        return DbContext.OrganizationMember.Update(organizationMember).Entity;
    }

    public async Task<IReadOnlyList<OrganizationMember>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Where(query => query.Organization.Id == organizationId)
            .AsSingleQuery()
            .Include(query => query.Customer)
            .ToListAsync(cancellationToken);
}
