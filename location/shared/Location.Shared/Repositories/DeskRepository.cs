using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Location.Shared.Database;
using Location.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Desk = Location.Shared.Database.Entities.Desk;
using Tag = Location.Shared.Database.Entities.Tag;

namespace Location.Shared.Repositories;

public interface IDeskRepository : IRepository<Desk>
{
    Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Desk Add(Desk desk);
    Desk Update(Desk desk);
    void RemoveRange(ICollection<Desk> desks);
    Desk Remove(Desk desk);

    Task<(PaginatedInfo, ICollection<Edge<Desk>>, int )> GetPaginatedDesksAsync(
        PaginationInputParam paginationInputParam,
        DeskSearchCriteria searchCriteria,
        ICollection<DeskOrder> orderByFields,
        CancellationToken cancellationToken);
}

internal static class DeskExtensions
{
    internal static IIncludableQueryable<Desk, IEnumerable<Tag>> AddDependentObjects(
        this IQueryable<Desk> originalQuery) =>
        originalQuery
            .Include(query => query.Location)
            .Include(query => query.Tags.Where(tag => !tag.DeletedAt.HasValue));

    internal static IQueryable<Desk> AddSearchCriteria(
        this IQueryable<Desk> query,
        DeskSearchCriteria searchCriteria)
    {
        query = query.Where(item => !item.DeletedAt.HasValue && item.Location.Id == searchCriteria.LocationId);

        if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
        {
            query = query.Where(item =>
                EF.Functions.ILike(item.Name, $"%{searchCriteria.NameContains}%") || item.Tags.Any(tag =>
                    EF.Functions.ILike(tag.Name, $"%{searchCriteria.NameContains}%")));
        }

        return query;
    }

    internal static IQueryable<Desk> AddSortingOrders(
        this IQueryable<Desk> originalQuery,
        ICollection<DeskOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return originalQuery.OrderBy(query => query.CreatedAt);
        }

        var orderByField = orderByFields.First();
        return orderByFields.Skip(1).Aggregate(orderByField.Field switch
        {
            DeskOrderField.Name => orderByField.Direction == OrderDirection.Ascending
                ? originalQuery.OrderBy(x => x.Name)
                : originalQuery.OrderByDescending(x => x.Name),
            _ => throw new ArgumentOutOfRangeException()
        }, (query, orderField) =>
            orderField.Field switch
            {
                DeskOrderField.Name => orderField.Direction == OrderDirection.Ascending
                    ? query.ThenBy(x => x.Name)
                    : query.ThenByDescending(x => x.Name),
                _ => throw new ArgumentOutOfRangeException()
            });
    }

    public static IQueryable<Desk> ApplyPaginationFilters(
        this IQueryable<Desk> query,
        PaginationInputParam paginationInputParam,
        ICollection<DeskOrder> orderByFields)
    {
        var orderByField = orderByFields.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            query = orderByField?.Field switch
            {
                DeskOrderField.Name => query.Where(item =>
                    orderByField.Direction == OrderDirection.Ascending
                        ? item.Name.CompareTo(paginationInputParam.After.FromCursor()) > 0
                        : item.Name.CompareTo(paginationInputParam.After.FromCursor()) < 0),
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
                DeskOrderField.Name => query.Where(item =>
                    orderByField.Direction == OrderDirection.Ascending
                        ? item.Name.CompareTo(paginationInputParam.Before.FromCursor()) < 0
                        : item.Name.CompareTo(paginationInputParam.Before.FromCursor()) > 0),
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

    public static ICollection<Edge<Desk>> ToEdges(this ICollection<Desk> items, ICollection<DeskOrder> orderByFields) =>
        items.Select(item => orderByFields.FirstOrDefault()?.Field switch
        {
            DeskOrderField.Name => new Edge<Desk>(item.Name.ToCursor(), item),
            null => new Edge<Desk>(item.CreatedAt.ToCursor(), item),
            _ => new Edge<Desk>(item.CreatedAt.ToCursor(), item)
        }).ToList();
}

public class DeskRepository(LocationDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<LocationDbContext, Desk>(dbContext), IDeskRepository
{
    public Desk Add(Desk desk)
    {
        var now = timeProvider.GetUtcNow();
        desk.CreatedAt = now;
        return DbContext.Desk.Add(desk).Entity;
    }

    public void RemoveRange(ICollection<Desk> desks)
    {
        var now = timeProvider.GetUtcNow();
        desks.ForEach(desk => desk.DeletedAt = now);
        DbContext.Desk.UpdateRange(desks);
    }

    public Desk Remove(Desk desk)
    {
        var now = timeProvider.GetUtcNow();
        desk.DeletedAt = now;
        return DbContext.Desk.Update(desk).Entity;
    }

    public Desk Update(Desk desk)
    {
        var now = timeProvider.GetUtcNow();
        desk.ModifiedAt = now;
        return DbContext.Desk.Update(desk).Entity;
    }

    public async Task<Desk?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Desk
            .Where(query => query.Id == id)
            .AddDependentObjects()
            .OrderBy(query => query.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<(PaginatedInfo, ICollection<Edge<Desk>>, int)> GetPaginatedDesksAsync(
        PaginationInputParam paginationInputParam,
        DeskSearchCriteria searchCriteria,
        ICollection<DeskOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var totalCount = await DbContext.Desk
            .AsQueryable()
            .AddSearchCriteria(searchCriteria)
            .CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], totalCount);
        }

        var (paginatedInfo, edges) = (await DbContext.Desk
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
