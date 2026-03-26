using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using Customer = Organization.Shared.Database.Entities.Customer;
using Identity = Organization.Shared.Database.Entities.Identity;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;

namespace Organization.Shared.Repositories;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    Task<OrganizationMember?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<OrganizationMember>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    OrganizationMember Add(OrganizationMember organizationMember);
    void AddRange(ICollection<OrganizationMember> organizationMembers);
    OrganizationMember Update(OrganizationMember organizationMember);
    void RemoveRange(ICollection<OrganizationMember> organizationMembers);

    Task<(PaginatedInfo, ICollection<Edge<OrganizationMember>>, int)> GetPaginatedOrganizationMembersAsync(
        PaginationInputParam paginationInputParam,
        OrganizationMemberSearchCriteria searchCriteria,
        ICollection<OrganizationMemberOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class OrganizationMemberExtensions
{
    extension(IQueryable<OrganizationMember> originalQuery)
    {
        internal IIncludableQueryable<OrganizationMember, ICollection<Identity>> AddDependentObjects() =>
            originalQuery
                .Include(query => query.Organization)
                .Include(query => query.Customer)
                .ThenInclude(query => query.Identities);

        internal IQueryable<OrganizationMember> AddSearchCriteria(OrganizationMemberSearchCriteria searchCriteria)
        {
            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
            {
                originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
            {
                originalQuery = originalQuery.Where(item =>
                    !item.DeletedAt.HasValue && item.Organization.CustomDomain != null &&
                    item.Organization.CustomDomain == searchCriteria.OrganizationCustomDomain);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item =>
                    (item.Customer.Name != null &&
                     EF.Functions.ILike(item.Customer.Name, $"%{searchCriteria.NameContains}%")) ||
                    (item.Customer.GivenName != null &&
                     EF.Functions.ILike(item.Customer.GivenName, $"%{searchCriteria.NameContains}%")) ||
                    (item.Customer.MiddleName != null &&
                     EF.Functions.ILike(item.Customer.MiddleName, $"%{searchCriteria.NameContains}%")) ||
                    (item.Customer.FamilyName != null &&
                     EF.Functions.ILike(item.Customer.FamilyName, $"%{searchCriteria.NameContains}%")));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
            {
                originalQuery = originalQuery.Where(item => item.Customer.Id == searchCriteria.CustomerId);
            }

            return originalQuery;
        }
    }
}

public class OrganizationMemberRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, OrganizationMember>(dbContext, timeProvider), IOrganizationMemberRepository
{
    public async Task<OrganizationMember?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .AddDependentObjects()
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<OrganizationMember>> GetByIdsAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .Where(query => ids.Contains(query.Id))
            .AddDependentObjects()
            .ToListAsync(cancellationToken);

    public OrganizationMember Add(OrganizationMember organizationMember)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMember.CreatedAt = now;
        return DbContext.OrganizationMember.Add(organizationMember).Entity;
    }

    public void AddRange(ICollection<OrganizationMember> organizationMembers)
    {
        var now = TimeProvider.GetUtcNow();
        organizationMembers.ForEach(organizationMember => organizationMember.CreatedAt = now);
        DbContext.OrganizationMember.AddRange(organizationMembers);
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

    public async Task<(PaginatedInfo, ICollection<Edge<OrganizationMember>>, int)>
        GetPaginatedOrganizationMembersAsync(
            PaginationInputParam paginationInputParam,
            OrganizationMemberSearchCriteria searchCriteria,
            ICollection<OrganizationMemberOrder> orderByFields,
            CancellationToken cancellationToken) =>
        await DbContext.OrganizationMember
            .AddSearchCriteria(searchCriteria)
            .AddDependentObjects()
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<OrganizationMember>> GetPaginationFields(ICollection<OrganizationMemberOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return
            [
                KeysetPaginationField<OrganizationMember>.Create(
                    nameof(Customer.Name),
                    query => query.Customer.Name,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                OrganizationMemberOrderField.Role => KeysetPaginationField<OrganizationMember>.Create(
                    nameof(OrganizationMember.Role),
                    query => query.Role,
                    orderField.Direction),
                OrganizationMemberOrderField.Status => KeysetPaginationField<OrganizationMember>.Create(
                    nameof(OrganizationMember.Status),
                    query => query.Status,
                    orderField.Direction),
                OrganizationMemberOrderField.Name => KeysetPaginationField<OrganizationMember>.Create(
                    nameof(Customer.Name),
                    query => query.Customer.Name,
                    orderField.Direction),
                OrganizationMemberOrderField.GivenName => KeysetPaginationField<OrganizationMember>.Create(
                    nameof(Customer.GivenName),
                    query => query.Customer.GivenName,
                    orderField.Direction),
                OrganizationMemberOrderField.MiddleName => KeysetPaginationField<OrganizationMember>.Create(
                    nameof(Customer.MiddleName),
                    query => query.Customer.MiddleName,
                    orderField.Direction),
                OrganizationMemberOrderField.FamilyName => KeysetPaginationField<OrganizationMember>.Create(
                    nameof(Customer.FamilyName),
                    query => query.Customer.FamilyName,
                    orderField.Direction),
                OrganizationMemberOrderField.PhoneNumber => KeysetPaginationField<OrganizationMember>.Create(
                    nameof(Customer.PhoneNumber),
                    query => query.Customer.PhoneNumber,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
