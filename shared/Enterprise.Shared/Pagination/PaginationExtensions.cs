using Enterprise.Shared.Database;
using Enterprise.Shared.Models;

namespace Enterprise.Shared.Pagination;

public static class PaginationExtensions
{
    public static (PaginatedInfo, ICollection<Edge<T>>, int) ToPaginated<T>(
        this IReadOnlyCollection<T> items,
        PaginationInputParam paginationInputParam) where T : EntityBase
    {
        var totalCount = items.Count;
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], 0);
        }

        IEnumerable<T> finalItems = items;

        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            var cursor = paginationInputParam.After.FromCursor();
            finalItems = finalItems.SkipWhile(booking => booking.Id != cursor).Skip(1);
        }
        else if (!string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
            var cursor = paginationInputParam.Before.FromCursor();
            finalItems = finalItems.TakeWhile(booking => booking.Id != cursor);
        }

        if (paginationInputParam.First is not null)
        {
            finalItems = finalItems.Take(paginationInputParam.First.Value).ToList();
        }
        else if (paginationInputParam.Last is not null)
        {
            finalItems = finalItems.Reverse().Take(paginationInputParam.Last.Value).Reverse().ToList();
        }

        var (paginatedInfo, edges) = finalItems
            .Select(item => new Edge<T>(item.Id.ToCursor(), item))
            .ToList()
            .GetPaginatedInfo(paginationInputParam);
        return (paginatedInfo, edges, totalCount);
    }

    private static (PaginatedInfo, ICollection<Edge<TEntityBase>>) GetPaginatedInfo<TEntityBase>(
        this ICollection<Edge<TEntityBase>> edges,
        PaginationInputParam paginationInputParam) where TEntityBase : EntityBase
    {
        var hasNextPage = false;
        var hasPreviousPage = false;
        if (paginationInputParam.First is null && string.IsNullOrWhiteSpace(paginationInputParam.After) &&
            paginationInputParam.Last is null && string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
        }
        else if (paginationInputParam.First is not null && string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            hasPreviousPage = false;
            if (edges.Count > paginationInputParam.First.Value)
            {
                hasNextPage = true;
                edges = edges.SkipLast(1).ToList();
            }
        }
        else if (paginationInputParam.First is null && !string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            hasPreviousPage = true;
            hasNextPage = false;
        }
        else if (paginationInputParam.First is not null && !string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            hasPreviousPage = true;
            if (edges.Count > paginationInputParam.First.Value)
            {
                hasNextPage = true;
                edges = edges.SkipLast(1).ToList();
            }
        }
        else if (paginationInputParam.Last is not null && string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
            hasNextPage = false;
            if (edges.Count > paginationInputParam.Last.Value)
            {
                hasPreviousPage = true;
                edges = edges.Skip(1).ToList();
            }
        }
        else if (paginationInputParam.Last is null && !string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
            hasPreviousPage = false;
            hasNextPage = true;
        }
        else if (paginationInputParam.Last is not null && !string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
            hasNextPage = true;
            if (edges.Count > paginationInputParam.Last.Value)
            {
                hasPreviousPage = true;
                edges = edges.SkipLast(1).ToList();
            }
        }

        var startCursor = edges.FirstOrDefault()?.Cursor;
        var endCursor = edges.LastOrDefault()?.Cursor;

        return (new PaginatedInfo(hasNextPage, hasPreviousPage, startCursor, endCursor), edges);
    }
}
