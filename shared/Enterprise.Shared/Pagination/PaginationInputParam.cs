namespace Enterprise.Shared.Pagination;

public class PaginationInputParam
{
    public PaginationInputParam(
        string? after,
        int? first,
        string? before,
        int? last)
    {
        if (!string.IsNullOrWhiteSpace(after) && !string.IsNullOrWhiteSpace(before))
        {
            throw new ArgumentException("`after` and `before` both can't have value at the same time");
        }

        if (first is not null && last is not null)
        {
            throw new ArgumentException("passing both `first` and `last` to paginate a connection is not supported.");
        }

        if ((first is not null || !string.IsNullOrWhiteSpace(after)) &&
            (last is not null || !string.IsNullOrWhiteSpace(before)))
        {
            throw new ArgumentException("mixing first and after with last and before is not supported.");
        }

        if (first < 0)
        {
            throw new ArgumentException("`first` on a connection cannot be less than zero.");
        }

        if (last < 0)
        {
            throw new ArgumentException("`last` on a connection cannot be less than zero.");
        }

        After = after;
        First = first;
        Before = before;
        Last = last;
    }

    public string? After { get; set; }
    public int? First { get; }
    public string? Before { get; set; }
    public int? Last { get; }
}
