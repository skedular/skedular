namespace Enterprise.Shared.Pagination;

public record PaginatedInfo(bool HasNextPage, bool HasPreviousPage, string? StartCursor, string? EndCursor);
