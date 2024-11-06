namespace Enterprise.Shared.Pagination;

public static class CursorExtensions
{
    public static string ToCursor(this string? val) => val.ToSafeString();
    public static string FromCursor(this string val) => val;
}
