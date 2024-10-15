namespace Enterprise.Shared.Sanitization;

public static class StringSanitization
{
    public static string[]? RemoveInvalidIds(this string[]? input) =>
        input?.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
}
