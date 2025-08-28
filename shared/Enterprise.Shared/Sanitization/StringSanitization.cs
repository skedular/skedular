namespace Enterprise.Shared.Sanitization;

public static class StringSanitization
{
    public static IEnumerable<string>? RemoveInvalidIds(this IEnumerable<string>? input) => input?.Where(id => !string.IsNullOrWhiteSpace(id));
}
