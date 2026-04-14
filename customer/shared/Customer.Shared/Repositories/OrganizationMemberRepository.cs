using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Customer.Shared.Repositories;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    Task<OrganizationMember> UpsertNakedAsync(
        string id,
        Organization organization,
        Database.Entities.Customer customer,
        CancellationToken cancellationToken);

    Task<OrganizationMember?> GetByIdAsync(string id, CancellationToken cancellationToken);
    OrganizationMember Add(OrganizationMember organizationMember);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(ICollection<OrganizationMember> organizationMembers);

    Task<ICollection<OrganizationMember>> GetByOrganizationIdAsync(
        string organizationId,
        CancellationToken cancellationToken);
}

public class OrganizationMemberRepository(CustomerDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<CustomerDbContext, OrganizationMember>(dbContext, timeProvider), IOrganizationMemberRepository
{
    public async Task<OrganizationMember> UpsertNakedAsync(
        string id,
        Organization organization,
        Database.Entities.Customer customer,
        CancellationToken cancellationToken)
    {
        await UpsertNakedAsync<Organization, Database.Entities.Customer>(id, organization, customer, cancellationToken);

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

    public void RemoveRange(ICollection<OrganizationMember> organizationMembers)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMembers.ForEach(organizationMember => organizationMember.DeletedAt = now);
        DbContext.OrganizationMember.UpdateRange(organizationMembers);
    }

    public OrganizationMember Update(OrganizationMember organizationMember)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMember.ModifiedAt = now;
        return DbContext.OrganizationMember.Update(organizationMember).Entity;
    }

    public async Task<ICollection<OrganizationMember>> GetByOrganizationIdAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Where(query => query.Organization.Id == organizationId)
            .AsSingleQuery()
            .Include(query => query.Customer)
            .ToListAsync(cancellationToken);
}
