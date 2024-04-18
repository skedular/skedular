using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization> UpsertNakedAsync(string id, CancellationToken cancellationToken);
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Organization Add(Organization organization);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

public class OrganizationRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, Organization>(dbContext), IOrganizationRepository
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
            .Include(query =>
                query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities)
            .Include(query => query.OrganizationOfferings
                .Where(organizationOffering => !organizationOffering.DeletedAt.HasValue)
                .OrderByDescending(organizationOffering => organizationOffering.End)
                .Take(1))
            .ThenInclude(query => query.OrganizationOfferingStripePaymentIntents)
            .Include(query => query.OrganizationStripePaymentMethods.Where(organizationStripePaymentMethod =>
                !organizationStripePaymentMethod.DeletedAt.HasValue))
            .OrderBy(query => query.Id)
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
