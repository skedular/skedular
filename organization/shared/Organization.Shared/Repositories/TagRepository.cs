using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Organization.Shared.Database;
using Organization.Shared.Models;
using Tag = Organization.Shared.Database.Entities.Tag;

namespace Organization.Shared.Repositories;

public interface ITagRepository : IRepository<Tag>
{
    Task<Tag?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Tag Add(Tag tag);
    Tag Update(Tag tag);
    void RemoveRange(ICollection<Tag> tags);
    Tag Remove(Tag tag);

    Task<(PaginatedInfo, ICollection<Edge<Tag>>, int )> GetPaginatedTagsAsync(
        PaginationInputParam paginationInputParam,
        TagSearchCriteria searchCriteria,
        ICollection<TagOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class TagExtensions
{
    internal static IIncludableQueryable<Tag, Database.Entities.Organization> AddDependentObjects(
        this IQueryable<Tag> originalQuery) =>
        originalQuery
            .Include(query => query.Organization);

    internal static IQueryable<Tag> AddSearchCriteria(
        this IQueryable<Tag> query,
        TagSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Organization.Id == searchCriteria.OrganizationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.Type))
        {
            query = query.Where(item => item.Type == searchCriteria.Type);
        }

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item => EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%"));
        }

        return query;
    }

    internal static IQueryable<Tag> AddSortingOrders(
        this IQueryable<Tag> originalQuery,
        ICollection<TagOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.Name).ThenBy(query => query.Id);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            TagOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            TagOrderField.Description => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Description)
                : originalQuery.OrderByDescending(x => x.Description),
            TagOrderField.TagType => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Type)
                : originalQuery.OrderByDescending(x => x.Type),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                TagOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            }).ThenBy(query => query.Id);
    }
}

public class TagRepository(OrganizationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<OrganizationDbContext, Tag>(dbContext), ITagRepository
{
    public Tag Add(Tag tag)
    {
        var now = timeProvider.GetUtcNow();
        tag.CreatedAt = now;
        return DbContext.Tag.Add(tag).Entity;
    }

    public void RemoveRange(ICollection<Tag> tags)
    {
        var now = timeProvider.GetUtcNow();
        tags.ForEach(tag => tag.DeletedAt = now);
        DbContext.Tag.UpdateRange(tags);
    }

    public Tag Remove(Tag tag)
    {
        var now = timeProvider.GetUtcNow();
        tag.DeletedAt = now;
        return DbContext.Tag.Update(tag).Entity;
    }

    public Tag Update(Tag tag)
    {
        var now = timeProvider.GetUtcNow();
        tag.ModifiedAt = now;
        return DbContext.Tag.Update(tag).Entity;
    }

    public async Task<Tag?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Tag
            .Where(query => query.Id == id)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

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
