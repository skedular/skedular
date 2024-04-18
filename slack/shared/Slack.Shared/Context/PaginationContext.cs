namespace Slack.Shared.Context;

public class PaginationContext
{
    public string? After { get; set; }
    public int? First { get; set; }
    public string? Before { get; set; }
    public int? Last { get; set; }
    public string? CurrentAfter { get; set; }
    public int? CurrentFirst { get; set; }
    public string? CurrentBefore { get; set; }
    public int? CurrentLast { get; set; }
}

public static class PaginationContextExtensions
{
    public static bool IsEmpty(this PaginationContext paginationContext) =>
        paginationContext.After is null &&
        paginationContext.First is null &&
        paginationContext.Before is null &&
        paginationContext.Last is null &&
        paginationContext.CurrentAfter is null &&
        paginationContext.CurrentFirst is null &&
        paginationContext.CurrentBefore is null &&
        paginationContext.CurrentLast is null;
}
