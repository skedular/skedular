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

        var hasPreviousPage = false;
        var hasNextPage = false;

        if (!string.IsNullOrWhiteSpace(paginationInputParam.After))
        {
            var cursor = paginationInputParam.After.FromCursor();
            if (items.First().Id != cursor)
            {
                hasPreviousPage = true;
            }

            items = items.SkipWhile(item => item.Id != cursor).Skip(1).ToList();
            if (paginationInputParam.First is not null)
            {
                if (items.Count > paginationInputParam.First.Value)
                {
                    hasNextPage = true;
                }
            }
        }
        else if (!string.IsNullOrWhiteSpace(paginationInputParam.Before))
        {
            var cursor = paginationInputParam.Before.FromCursor();
            if (items.Last().Id != cursor)
            {
                hasNextPage = true;
            }

            items = items.TakeWhile(item => item.Id != cursor).ToList();
            if (paginationInputParam.Last is not null)
            {
                if (items.Count > paginationInputParam.Last.Value)
                {
                    hasPreviousPage = true;
                }
            }
        }
        else
        {
            if (paginationInputParam.First is not null)
            {
                if (items.Count > paginationInputParam.First.Value)
                {
                    hasNextPage = true;
                }
            }
            else if (paginationInputParam.Last is not null)
            {
                if (items.Count > paginationInputParam.Last.Value)
                {
                    hasPreviousPage = true;
                }
            }
        }

        if (paginationInputParam.First is not null)
        {
            items = items.Take(paginationInputParam.First.Value).ToList();
        }
        else if (paginationInputParam.Last is not null)
        {
            items = items.Reverse().Take(paginationInputParam.Last.Value).Reverse().ToList();
        }

        var edges = items.Select(item => new Edge<T>(item.Id.ToCursor(), item)).ToList();
        var startCursor = edges.FirstOrDefault()?.Cursor;
        var endCursor = edges.LastOrDefault()?.Cursor;

        var paginatedInfo = new PaginatedInfo(hasNextPage, hasPreviousPage, startCursor, endCursor);
        return (paginatedInfo, edges, totalCount);
    }
}
