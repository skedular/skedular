using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Tag = Location.Shared.Database.Entities.Tag;

namespace Location.Shared.Repositories;

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
    internal static IIncludableQueryable<Tag, Database.Entities.Location> AddDependentObjects(
        this IQueryable<Tag> originalQuery) =>
        originalQuery
            .Include(query => query.Location);

    internal static IQueryable<Tag> AddSearchCriteria(
        this IQueryable<Tag> query,
        TagSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Location.Id == searchCriteria.LocationId);

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
            return originalQuery.OrderBy(query => query.CreatedAt);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            TagOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                TagOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    public static IQueryable<Tag> ApplyPaginationFilters(
        this IQueryable<Tag> query,
        PaginationInputParam paginationInputParam,
        ICollection<TagOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                TagOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0)
                    : query.Where(item =>
                        item.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
                null => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0),
                _ => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.After.FromCursorToDateTimeOffset()) > 0)
            };
        }
        else if (!string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
            query = orderByField?.Field switch
            {
                TagOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                    ? query.Where(item =>
                        item.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0)
                    : query.Where(item =>
                        item.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
                null => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0),
                _ => query.Where(item =>
                    item.CreatedAt.CompareTo(paginationInputParam.Before.FromCursorToDateTimeOffset()) < 0)
            };
        }

        if (paginationInputParam.First is not null)
        {
            query = query.Take(paginationInputParam.First.Value + 1);
        }
        else if (paginationInputParam.Last is not null)
        {
            query = query.Take(paginationInputParam.Last.Value + 1);
        }

        return query;
    }

    public static ICollection<Edge<Tag>> ToEdges(this ICollection<Tag> items, ICollection<TagOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            TagOrderField.Name => new Edge<Tag>(item.Name.ToCursor(), item),
            null => new Edge<Tag>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<Tag>(item.CreatedAt.ToCursor(), item)
        }).ToList();
}

public class TagRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Tag>(dbContext), ITagRepository
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
        CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.Tag.AsQueryable().AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.Tag
                .AsQueryable()
                .AddSearchCriteria(searchCriteria)
                .AddSortingOrders(orderByFields)
                .ApplyPaginationFilters(paginationInputParam, orderByFields)
                .AddDependentObjects()
                .ToListAsync(cancellationToken))
            .ToEdges(orderByFields)
            .GetPaginatedInfo(paginationInputParam);
        return (paginatedInfo, edges, totalCount);
    }
}
