namespace Enterprise.Shared;

public static class StringExtensions
{
    public static string ToSafeString(this string? str) => string.IsNullOrWhiteSpace(str) ? string.Empty : str;

    public static string Truncate(this string? content, int length)
    {
        var str = string.IsNullOrWhiteSpace(content) ? string.Empty : content;

        return string.IsNullOrWhiteSpace(str) || str.Length <= length ? str : str[..length];
    }
}
