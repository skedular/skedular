using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Sanitization;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using IndustryMainCategory = Organization.Shared.Database.Entities.IndustryMainCategory;
using OrganizationStripePaymentMethod = Organization.Shared.Database.Entities.OrganizationStripePaymentMethod;

namespace Organization.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Database.Entities.Organization>
{
    Task<Database.Entities.Organization?> GetByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    Task<Database.Entities.Organization?> GetByIdOrCustomDomainUntrackedAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    Task<Database.Entities.Organization?> GetPublicByIdOrCustomDomainAsync(string? id, string? customDomain, CancellationToken cancellationToken);

    Task<IReadOnlyList<Database.Entities.Organization>> GetByIdsOrCustomDomainsAsync(
        IReadOnlyList<string>? ids,
        IReadOnlyList<string>? customDomains,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Database.Entities.Organization>> GetMinimalOrganizationByCustomerIdUntrackedAsync(
        string customerId,
        CancellationToken cancellationToken);

    Task<Database.Entities.Organization?> GetByAzureTenantIdUntrackedAsync(string azureTenantId, CancellationToken cancellationToken);
    Task<Database.Entities.Organization?> GetByXeroTenantIdUntrackedAsync(string tenantId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Database.Entities.Organization>> GetAllAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Database.Entities.Organization>> GetAllUntrackedAsync(CancellationToken cancellationToken);
    Database.Entities.Organization Add(Database.Entities.Organization organization);
    Database.Entities.Organization Update(Database.Entities.Organization organization);
    Database.Entities.Organization Remove(Database.Entities.Organization organization);
    void ClearTrackedEntities();

    Task<(PaginatedInfo, IReadOnlyList<Edge<Database.Entities.Organization>>, int)> GetPaginatedOrganizationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        IReadOnlyList<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken);
}

public static class OrganizationExtensions
{
    extension(IQueryable<Database.Entities.Organization> originalQuery)
    {
        public IIncludableQueryable<Database.Entities.Organization, OrganizationStripePaymentMethod> AddDependentObjects(bool isTracked,
            bool includeAllOfferings)
        {
            var updatedQuery = (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
                .Include(query => query.OrganizationSsoSettings)
                .Include(query => query.OrganizationTaxDetails)
                .Include(query => query.OrganizationXeroConnection)
                .Include(query => query.AzureTenants.Where(azureTenant => !azureTenant.DeletedAt.HasValue))
                .ThenInclude(query => query.AzureTenantMembers.Where(azureTenantMember => !azureTenantMember.DeletedAt.HasValue))
                .Include(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
                .ThenInclude(query => query.Customer)
                .ThenInclude(query => query.Identities)
                .Include(query => query.TermsOfUse)
                .Include(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
                .Include(query => query.PhysicalAddress)
                .Include(query => query.BillingDetails)
                .Include(query => query.OrganizationStripeCustomer)
                .Include(query =>
                    query.OrganizationStripePaymentMethods.Where(organizationStripePaymentMethod =>
                        !organizationStripePaymentMethod.DeletedAt.HasValue))
                .Include(query =>
                    query.OrganizationStripeConnectAccounts.Where(organizationStripeConnectAccount =>
                        !organizationStripeConnectAccount.DeletedAt.HasValue))
                .ThenInclude(query => query.OrganizationStripeConnectAccountAuthorization)
                .Include(query => query.OrganizationBankAccounts.Where(organizationBankAccount => !organizationBankAccount.DeletedAt.HasValue))
                .Include(query => query.IndustrySubCategories)
                .ThenInclude(query => query.IndustryMainCategory);

            return includeAllOfferings
                ? updatedQuery
                    .Include(query => query.OrganizationOfferings.OrderByDescending(organizationOffering => organizationOffering.End))
                    .ThenInclude(query => query.OrganizationOfferingActiveMembers)
                    .ThenInclude(query => query.OrganizationMember)
                    .ThenInclude(query => query.Customer)
                    .Include(query => query.OrganizationOfferings.OrderByDescending(organizationOffering => organizationOffering.End))
                    .ThenInclude(query => query.OrganizationStripePaymentIntent)
                    .ThenInclude(query => query!.OrganizationStripePaymentMethod)
                : updatedQuery
                    .Include(query => query.OrganizationOfferings
                        .Where(organizationOffering => !organizationOffering.DeletedAt.HasValue)
                        .OrderByDescending(organizationOffering => organizationOffering.End).Take(1))
                    .ThenInclude(query => query.OrganizationOfferingActiveMembers)
                    .ThenInclude(query => query.OrganizationMember)
                    .ThenInclude(query => query.Customer)
                    .Include(query => query.OrganizationOfferings
                        .Where(organizationOffering => !organizationOffering.DeletedAt.HasValue)
                        .OrderByDescending(organizationOffering => organizationOffering.End).Take(1))
                    .ThenInclude(query => query.OrganizationStripePaymentIntent)
                    .ThenInclude(query => query!.OrganizationStripePaymentMethod);
        }

        public IIncludableQueryable<Database.Entities.Organization, IndustryMainCategory> AddPublicDependentObjects() =>
            originalQuery
                .AsNoTrackingWithIdentityResolution()
                .Include(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue))
                .Include(query => query.PhysicalAddress)
                .Include(query => query.IndustrySubCategories)
                .ThenInclude(query => query.IndustryMainCategory);

        public IQueryable<Database.Entities.Organization> AddSearchCriteria(OrganizationSearchCriteria searchCriteria)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(searchCriteria.CustomerId);

            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue);
            originalQuery = originalQuery
                .Where(item => item.OrganizationMembers
                    .Any(organizationMember =>
                        !organizationMember.DeletedAt.HasValue && organizationMember.Customer.Id == searchCriteria.CustomerId));

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
            }

            return originalQuery;
        }
    }
}

public class OrganizationRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Database.Entities.Organization>(dbContext, timeProvider), IOrganizationRepository
{
    public void ClearTrackedEntities() => DbContext.ChangeTracker.Clear();

    public Database.Entities.Organization Add(Database.Entities.Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.CreatedAt = now;
        return DbContext.Organization.Add(organization).Entity;
    }

    public Database.Entities.Organization Update(Database.Entities.Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.ModifiedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public async Task<Database.Entities.Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects(true, false)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            return await DbContext.Organization
                .AddDependentObjects(true, false)
                .FirstOrDefaultAsync(
                    query => query.CustomDomain != null && query.CustomDomain == customDomain,
                    cancellationToken);
        }

        throw new OrganizationLookupRequiresIdOrCustomDomainException();
    }

    public async Task<Database.Entities.Organization?> GetByIdOrCustomDomainUntrackedAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects(false, false)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            return await DbContext.Organization
                .AddDependentObjects(false, false)
                .FirstOrDefaultAsync(
                    query => query.CustomDomain != null && query.CustomDomain == customDomain,
                    cancellationToken);
        }

        throw new OrganizationLookupRequiresIdOrCustomDomainException();
    }

    public async Task<Database.Entities.Organization?> GetPublicByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddPublicDependentObjects()
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(customDomain))
        {
            return await DbContext.Organization
                .AddPublicDependentObjects()
                .FirstOrDefaultAsync(
                    query => query.CustomDomain != null && query.CustomDomain == customDomain,
                    cancellationToken);
        }

        throw new OrganizationLookupRequiresIdOrCustomDomainException();
    }

    public async Task<IReadOnlyList<Database.Entities.Organization>> GetByIdsOrCustomDomainsAsync(
        IReadOnlyList<string>? ids,
        IReadOnlyList<string>? customDomains,
        CancellationToken cancellationToken)
    {
        if (ids is not null && ids.RemoveInvalidIds().Any() && customDomains is not null && customDomains.RemoveInvalidIds().Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();
            customDomains = customDomains.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id) || (query.CustomDomain != null && customDomains.Contains(query.CustomDomain)))
                .AddDependentObjects(true, false)
                .ToListAsync(cancellationToken);
        }

        if (ids is not null && ids.RemoveInvalidIds().Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id))
                .AddDependentObjects(true, false)
                .ToListAsync(cancellationToken);
        }

        if (customDomains is not null && customDomains.RemoveInvalidIds().Any())
        {
            customDomains = customDomains.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => query.CustomDomain != null && customDomains.Contains(query.CustomDomain))
                .AddDependentObjects(true, false)
                .ToListAsync(cancellationToken);
        }

        throw new OrganizationLookupRequiresIdsOrCustomDomainsException();
    }

    public async Task<IReadOnlyList<Database.Entities.Organization>> GetMinimalOrganizationByCustomerIdUntrackedAsync(
        string customerId,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue && query.OrganizationMembers.Select(item => item.Customer.Id).Contains(customerId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<Database.Entities.Organization?> GetByAzureTenantIdUntrackedAsync(string azureTenantId, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddDependentObjects(false, false)
            .FirstOrDefaultAsync(
                query => !query.DeletedAt.HasValue && query.AzureTenants.Any(azureTenant => azureTenant.Id == azureTenantId),
                cancellationToken);

    public async Task<Database.Entities.Organization?> GetByXeroTenantIdUntrackedAsync(string tenantId, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddDependentObjects(false, false)
            .FirstOrDefaultAsync(
                query =>
                    !query.DeletedAt.HasValue &&
                    query.OrganizationXeroConnection != null &&
                    query.OrganizationXeroConnection.TenantId == tenantId,
                cancellationToken);

    public async Task<IReadOnlyList<Database.Entities.Organization>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects(true, false)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Database.Entities.Organization>> GetAllUntrackedAsync(CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects(false, false)
            .ToListAsync(cancellationToken);

    public Database.Entities.Organization Remove(Database.Entities.Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Database.Entities.Organization>>, int)> GetPaginatedOrganizationsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        IReadOnlyList<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects(false, false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<Database.Entities.Organization>> GetPaginationFields(IReadOnlyList<OrganizationOrder> orderByFields)
    {
        if (!orderByFields.Any())
        {
            return
            [
                KeysetPaginationField<Database.Entities.Organization>.Create(
                    nameof(Database.Entities.Organization.Name),
                    query => query.Name,
                    OrderDirection.Ascending),
            ];
        }

        return
        [
            .. orderByFields.Select(orderField => orderField.Field switch
            {
                OrganizationOrderField.Name => KeysetPaginationField<Database.Entities.Organization>.Create(
                    nameof(Database.Entities.Organization.Name),
                    query => query.Name,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            }),
        ];
    }
}
