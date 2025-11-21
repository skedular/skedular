namespace Enterprise.Shared.Sanitization;

public static class StringSanitization
{
    extension(IEnumerable<string>? input)
    {
        public IEnumerable<string>? RemoveInvalidIds() => input?.Where(id => !string.IsNullOrWhiteSpace(id));
    }
}
