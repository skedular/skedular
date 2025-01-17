using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Payment.Shared.Database;
using Payment.Shared.Database.Entities;

namespace Payment.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<Organization?> GetByIdAsync(
        string id,
        bool includeDeletedOrganizationMembers,
        bool includeAllOfferings,
        CancellationToken cancellationToken);

    Organization Add(Organization organization);
    Organization Update(Organization organization);
    Organization Remove(Organization organization);
}

internal static class OrganizationExtensions
{
    internal static IIncludableQueryable<Organization, IEnumerable<OrganizationStripePaymentMethod>>
        AddDependentObjects(
            this IQueryable<Organization> originalQuery,
            bool includeDeletedOrganizationMembers,
            bool includeAllOfferings)
    {
        var updatedQuery = originalQuery
            .Include(query => query.OrganizationMembers.Where(organizationMember =>
                includeDeletedOrganizationMembers || !organizationMember.DeletedAt.HasValue))
            .ThenInclude(query => query.Customer)
            .ThenInclude(query => query.Identities);

        return includeAllOfferings
            ? updatedQuery
                .Include(query => query.OrganizationOfferings
                    .OrderByDescending(organizationOffering => organizationOffering.End))
                .ThenInclude(query => query.OrganizationOfferingStripePaymentIntents)
                .Include(query => query.OrganizationStripePaymentMethods.Where(organizationStripePaymentMethod =>
                    !organizationStripePaymentMethod.DeletedAt.HasValue))
            : updatedQuery
                .Include(query => query.OrganizationOfferings
                    .Where(organizationOffering => !organizationOffering.DeletedAt.HasValue)
                    .OrderByDescending(organizationOffering => organizationOffering.End)
                    .Take(1))
                .ThenInclude(query => query.OrganizationOfferingStripePaymentIntents)
                .Include(query => query.OrganizationStripePaymentMethods.Where(organizationStripePaymentMethod =>
                    !organizationStripePaymentMethod.DeletedAt.HasValue));
    }
}

public class OrganizationRepository(PaymentDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<PaymentDbContext, Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public async Task<Organization?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await GetByIdAsync(id, false, false, cancellationToken);

    public async Task<Organization?> GetByIdAsync(
        string id,
        bool includeDeletedOrganizationMembers,
        bool includeAllOfferings,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddDependentObjects(includeDeletedOrganizationMembers, includeAllOfferings)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public Organization Add(Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Organization.Add(organization).Entity;
    }

    public Organization Update(Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public Organization Remove(Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }
}
