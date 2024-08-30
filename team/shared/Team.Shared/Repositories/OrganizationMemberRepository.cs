using Enterprise.Shared;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Team.Shared.Database;
using Team.Shared.Database.Entities;

namespace Team.Shared.Repositories;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    Task<OrganizationMember> UpsertNakedAsync(
        string id,
        Organization organization,
        Customer customer,
        CancellationToken cancellationToken);

    Task<OrganizationMember?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationMember Add(OrganizationMember organizationMember);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(ICollection<OrganizationMember> organizationMembers);
}

public class OrganizationMemberRepository(TeamDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<TeamDbContext, OrganizationMember>(dbContext), IOrganizationMemberRepository
{
    public async Task<OrganizationMember> UpsertNakedAsync(
        string id,
        Organization organization,
        Customer customer,
        CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var now = timeProvider.GetUtcNow();
        return DbContext.OrganizationMember
            .Add(new OrganizationMember { Id = id, Organization = organization, Customer = customer, CreatedAt = now })
            .Entity;
    }

    public async Task<OrganizationMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Where(query => query.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

    public OrganizationMember Add(OrganizationMember organizationMember)
    {
        var now = timeProvider.GetUtcNow();
        organizationMember.CreatedAt = now;
        return DbContext.OrganizationMember.Add(organizationMember).Entity;
    }

    public void RemoveRange(ICollection<OrganizationMember> organizationMembers)
    {
        var now = timeProvider.GetUtcNow();
        organizationMembers.ForEach(organizationMember => organizationMember.DeletedAt = now);
        DbContext.OrganizationMember.UpdateRange(organizationMembers);
    }

    public OrganizationMember Update(OrganizationMember organizationMember)
    {
        var now = timeProvider.GetUtcNow();
        organizationMember.ModifiedAt = now;
        return DbContext.OrganizationMember.Update(organizationMember).Entity;
    }
}
