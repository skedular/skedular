using System.Text.Json;

namespace Enterprise.Shared.Pagination;

public static class CursorExtensions
{
    public static string ToCursor(this string? val) => val.ToSafeString();
    public static string ToCursor<TEnum>(this TEnum val) where TEnum : struct => val.ToString().ToCursor();
    public static string FromCursor(this string val) => val;

    public static string ToCursor(this DateTimeOffset val) =>
        JsonSerializer.Serialize(val).ToCursor();

    public static DateTimeOffset FromCursorToDateTimeOffset(this string val) =>
        JsonSerializer.Deserialize<DateTimeOffset>(val.FromCursor());

    public static TEnum FromCursorToEnum<TEnum>(this string val) where TEnum : struct =>
        Enum.Parse<TEnum>(val.FromCursor());
}
