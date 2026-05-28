using Customer.Shared.Database;
using Customer.Shared.Database.Entities;
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
    Task<OrganizationMember?> GetByIdWithCustomerAsync(string id, CancellationToken cancellationToken);
    OrganizationMember Add(OrganizationMember organizationMember);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(IEnumerable<OrganizationMember> organizationMembers);

    Task<IReadOnlyList<OrganizationMember>> GetByOrganizationIdAsync(
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

    /// <summary>
    ///     Loads an organization member together with its linked customer record.
    /// </summary>
    /// <param name="id">The organization member identifier to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token for the database query.</param>
    /// <returns>The organization member with <c>Customer</c> populated, or <see langword="null" /> when the member does not exist.</returns>
    /// <remarks>
    ///     This focused lookup was added to replace the old shared specification used by subscriber flows that need both the member and the linked customer
    ///     in a single repository call.
    /// </remarks>
    public async Task<OrganizationMember?> GetByIdWithCustomerAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Include(query => query.Customer)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

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

    public async Task<IReadOnlyList<OrganizationMember>> GetByOrganizationIdAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Where(query => query.Organization.Id == organizationId)
            .AsSingleQuery()
            .Include(query => query.Customer)
            .ToListAsync(cancellationToken);
}
