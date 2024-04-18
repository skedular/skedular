using Enterprise.Shared.Database;
using Enterprise.Shared.Models;

namespace Enterprise.Shared.Pagination;

public static class PaginationExtensions
{
    public static (PaginatedInfo, ICollection<Edge<TEntityBase>>) GetPaginatedInfo<TEntityBase>(
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
