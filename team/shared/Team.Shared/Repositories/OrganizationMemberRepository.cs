using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    Task<OrganizationMember> UpsertNakedAsync(string id, Organization organization, Customer customer, CancellationToken cancellationToken);
    Task<OrganizationMember?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationMember Add(OrganizationMember organizationMember);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(IEnumerable<OrganizationMember> organizationMembers);
    Task<IReadOnlyList<OrganizationMember>> GetByOrganizationIdAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationMemberRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, OrganizationMember>(dbContext, timeProvider), IOrganizationMemberRepository
{
    public async Task<OrganizationMember> UpsertNakedAsync(
        string id,
        Organization organization,
        Customer customer,
        CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization, Customer>(id, organization, customer, cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task<OrganizationMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember.FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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
