namespace Enterprise.Shared.Pagination;

public static class CursorExtensions
{
    extension(string? val)
    {
        public string ToCursor() => val.ToSafeString();
    }

    extension(string val)
    {
        public string FromCursor() => val;
    }
}
