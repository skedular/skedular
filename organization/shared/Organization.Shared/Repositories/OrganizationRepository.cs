using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Sanitization;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using Team = Organization.Shared.Database.Entities.Team;

namespace Organization.Shared.Repositories;

public interface IOrganizationRepository : IRepository<Database.Entities.Organization>
{
    Task<Database.Entities.Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Organization>> GetByIdsOrUniqueAlphanumericNamesAsync(
        ICollection<string>? ids,
        ICollection<string>? uniqueAlphanumericNames,
        CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Organization>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken);
    Task<Database.Entities.Organization?> GetByAzureTenantIdAsync(string azureTenantId, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Organization>> GetAllAsync(CancellationToken cancellationToken);
    Database.Entities.Organization Add(Database.Entities.Organization organization);
    Database.Entities.Organization Update(Database.Entities.Organization organization);
    Database.Entities.Organization Remove(Database.Entities.Organization organization);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Organization>>, int)> GetPaginatedOrganizationsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        ICollection<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class OrganizationExtensions
{
    internal static IIncludableQueryable<Database.Entities.Organization, ICollection<Team>> AddDependentObjects(
        this IQueryable<Database.Entities.Organization> originalQuery,
        bool includeAllOfferings)
    {
        var updatedQuery = originalQuery
            .Include(query => query.OrganizationSsoSettings)
            .Include(query => query.OrganizationTaxDetails)
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
                query.OrganizationStripePaymentMethods.Where(organizationStripePaymentMethod => !organizationStripePaymentMethod.DeletedAt.HasValue))
            .Include(query =>
                query.OrganizationStripeConnectAccounts.Where(organizationStripeConnectAccount =>
                    !organizationStripeConnectAccount.DeletedAt.HasValue))
            .ThenInclude(query => query.OrganizationStripeConnectAccountAuthorization)
            .Include(query => query.OrganizationBankAccounts.Where(organizationBankAccount => !organizationBankAccount.DeletedAt.HasValue));

        return includeAllOfferings
            ? updatedQuery
                .Include(query => query.OrganizationOfferings.OrderByDescending(organizationOffering => organizationOffering.End))
                .ThenInclude(query => query.OrganizationOfferingActiveMembers)
                .ThenInclude(query => query.OrganizationMember)
                .ThenInclude(query => query.Customer)
                .Include(query => query.OrganizationOfferings.OrderByDescending(organizationOffering => organizationOffering.End))
                .ThenInclude(query => query.OrganizationStripePaymentIntent)
                .ThenInclude(query => query!.OrganizationStripePaymentMethod)
                .Include(query => query.IndustrySubCategories)
                .ThenInclude(query => query.IndustryMainCategory)
                .Include(query => query.Locations)
                .Include(query => query.Teams)
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
                .ThenInclude(query => query!.OrganizationStripePaymentMethod)
                .Include(query => query.IndustrySubCategories)
                .ThenInclude(query => query.IndustryMainCategory)
                .Include(query => query.Locations)
                .Include(query => query.Teams);
    }

    internal static IQueryable<Database.Entities.Organization> AddSearchCriteria(
        this IQueryable<Database.Entities.Organization> query,
        OrganizationSearchCriteria searchCriteria)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchCriteria.CustomerId);

        query = query.Where(item => !item.DeletedAt.HasValue);
        query = query
            .Where(item => item.OrganizationMembers
                .Any(organizationMember => !organizationMember.DeletedAt.HasValue && organizationMember.Customer.Id == searchCriteria.CustomerId));

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        return query;
    }

    internal static IQueryable<Database.Entities.Organization> AddSortingOrders(
        this IQueryable<Database.Entities.Organization> originalQuery,
        ICollection<OrganizationOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            OrganizationOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                OrganizationOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class OrganizationRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Database.Entities.Organization>(dbContext, timeProvider), IOrganizationRepository
{
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

    public async Task<Database.Entities.Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            return await DbContext.Organization
                .AddDependentObjects(false)
                .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(uniqueAlphanumericName))
        {
            return await DbContext.Organization
                .AddDependentObjects(false)
                .FirstOrDefaultAsync(
                    query => query.UniqueAlphanumericName != null && query.UniqueAlphanumericName == uniqueAlphanumericName,
                    cancellationToken);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
    }

    public async Task<ICollection<Database.Entities.Organization>> GetByIdsOrUniqueAlphanumericNamesAsync(
        ICollection<string>? ids,
        ICollection<string>? uniqueAlphanumericNames,
        CancellationToken cancellationToken)
    {
        if (ids is not null && ids.RemoveInvalidIds()!.Any() &&
            uniqueAlphanumericNames is not null && uniqueAlphanumericNames.RemoveInvalidIds()!.Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();
            uniqueAlphanumericNames = uniqueAlphanumericNames.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query =>
                    ids.Contains(query.Id) ||
                    (query.UniqueAlphanumericName != null && uniqueAlphanumericNames.Contains(query.UniqueAlphanumericName)))
                .AddDependentObjects(false)
                .ToListAsync(cancellationToken);
        }

        if (ids is not null && ids.RemoveInvalidIds()!.Any())
        {
            ids = ids.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => ids.Contains(query.Id))
                .AddDependentObjects(false)
                .ToListAsync(cancellationToken);
        }

        if (uniqueAlphanumericNames is not null && uniqueAlphanumericNames.RemoveInvalidIds()!.Any())
        {
            uniqueAlphanumericNames = uniqueAlphanumericNames.RemoveInvalidIds().ToSafeCollection();

            return await DbContext.Organization
                .Where(query => query.UniqueAlphanumericName != null && uniqueAlphanumericNames.Contains(query.UniqueAlphanumericName))
                .AddDependentObjects(false)
                .ToListAsync(cancellationToken);
        }

        throw new InvalidOperationException("Either ids or uniqueAlphanumericNames must be provided.");
    }

    public async Task<ICollection<Database.Entities.Organization>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue && query.OrganizationMembers.Select(item => item.Customer.Id).Contains(customerId))
            .AddDependentObjects(false)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<Database.Entities.Organization?> GetByAzureTenantIdAsync(string azureTenantId, CancellationToken cancellationToken) =>
        await DbContext.Organization
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(
                query => !query.DeletedAt.HasValue && query.AzureTenants.Any(azureTenant => azureTenant.Id == azureTenantId),
                cancellationToken);

    public async Task<ICollection<Database.Entities.Organization>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Organization
            .Where(query => !query.DeletedAt.HasValue)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken);

    public Database.Entities.Organization Remove(Database.Entities.Organization organization)
    {
        var now = TimeProvider.GetUtcNow();
        organization.DeletedAt = now;
        return DbContext.Organization.Update(organization).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Organization>>, int)> GetPaginatedOrganizationsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        ICollection<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Organization
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects(false)
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
