using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using Tag = Organization.Shared.Database.Entities.Tag;

namespace Organization.Shared.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Tag>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Tag Add(Tag tag);
    Tag Update(Tag tag);
    void RemoveRange(ICollection<Tag> tags);
    Tag Remove(Tag tag);

    Task<(PaginatedInfo, ICollection<Edge<Tag>>, int)> GetPaginatedTagsAsync(
        PaginationInputParam paginationInputParam,
        TagSearchCriteria searchCriteria,
        ICollection<TagOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class TagExtensions
{
    extension(IQueryable<Tag> originalQuery)
    {
        internal IIncludableQueryable<Tag, Database.Entities.Organization> AddDependentObjects() =>
            originalQuery.Include(query => query.Organization);

        internal IQueryable<Tag> AddSearchCriteria(TagSearchCriteria searchCriteria)
        {
            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
            {
                originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationUniqueAlphanumericName))
            {
                originalQuery = originalQuery.Where(item =>
                    !item.DeletedAt.HasValue && item.Organization.UniqueAlphanumericName != null &&
                    item.Organization.UniqueAlphanumericName == searchCriteria.OrganizationUniqueAlphanumericName);
            }

            if (searchCriteria.Types.Count != 0)
            {
                originalQuery = originalQuery.Where(item => searchCriteria.Types.Contains(item.Type));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
            }

            return originalQuery;
        }

        internal IQueryable<Tag> AddSortingOrders(ICollection<TagOrder> orderByFields)
        {
            if (orderByFields.Count == 0)
            {
                return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
            }

            var orderByField = orderByFields.First();
            return orderByFields.Skip(1).Aggregate(orderByField.Field switch
            {
                OrganizationTagOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? originalQuery.OrderBy(x => x.Name)
                    : originalQuery.OrderByDescending(x => x.Name),
                OrganizationTagOrderField.Description => orderByField.Direction == OrderDirection.Ascending
                    ? originalQuery.OrderBy(x => x.Description)
                    : originalQuery.OrderByDescending(x => x.Description),
                OrganizationTagOrderField.Type => orderByField.Direction == OrderDirection.Ascending
                    ? originalQuery.OrderBy(x => x.Type)
                    : originalQuery.OrderByDescending(x => x.Type),
                _ => throw new ArgumentOutOfRangeException()
            }, (query, orderField) =>
                orderField.Field switch
                {
                    OrganizationTagOrderField.Name => orderField.Direction == OrderDirection.Ascending
                        ? query.ThenBy(x => x.Name)
                        : query.ThenByDescending(x => x.Name),
                    OrganizationTagOrderField.Description => orderField.Direction == OrderDirection.Ascending
                        ? query.ThenBy(x => x.Description)
                        : query.ThenByDescending(x => x.Description),
                    OrganizationTagOrderField.Type => orderField.Direction == OrderDirection.Ascending
                        ? query.ThenBy(x => x.Type)
                        : query.ThenByDescending(x => x.Type),
                    _ => throw new ArgumentOutOfRangeException()
                }).ThenBy(query => query.Id);
        }
    }
}

public class TagRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Tag>(dbContext, timeProvider), ITagRepository
{
    public async Task<Tag?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Tag.AddDependentObjects().FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Tag>> GetByIdsAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Tag.Where(query => ids.Contains(query.Id)).AddDependentObjects().ToListAsync(cancellationToken);

    public Tag Add(Tag tag)
    {
        var now = TimeProvider.GetUtcNow();
        tag.CreatedAt = now;
        return DbContext.Tag.Add(tag).Entity;
    }

    public void RemoveRange(ICollection<Tag> tags)
    {
        var now = TimeProvider.GetUtcNow();
        tags.ForEach(tag => tag.DeletedAt = now);
        DbContext.Tag.UpdateRange(tags);
    }

    public Tag Remove(Tag tag)
    {
        var now = TimeProvider.GetUtcNow();
        tag.DeletedAt = now;
        return DbContext.Tag.Update(tag).Entity;
    }

    public Tag Update(Tag tag)
    {
        var now = TimeProvider.GetUtcNow();
        tag.ModifiedAt = now;
        return DbContext.Tag.Update(tag).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Tag>>, int)> GetPaginatedTagsAsync(
        PaginationInputParam paginationInputParam,
        TagSearchCriteria searchCriteria,
        ICollection<TagOrder> orderByFields,
        CancellationToken cancellationToken) =>
        (await DbContext.Tag
            .AddSearchCriteria(searchCriteria)
            .AddSortingOrders(orderByFields)
            .AddDependentObjects()
            .ToListAsync(cancellationToken))
        .ToPaginated(paginationInputParam);
}
