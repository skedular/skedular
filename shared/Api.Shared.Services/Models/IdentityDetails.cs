namespace Api.Shared.Services.Models;

public interface IIdentityDetails
{
    string? Email { get; set; }
    bool? EmailVerified { get; set; }
}

public static class IdentityDetailsExtensions
{
    public static ICollection<string> ToEmails(this IEnumerable<string?> src) =>
        src
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Select(item => item!.ToLowerInvariant())
            .Distinct()
            .ToList();

    public static ICollection<string> ToEmails<T>(this IEnumerable<T> src) where T : IIdentityDetails =>
        src.Select(item => item.Email).ToEmails();

    public static string ToStringEmails<T>(this IEnumerable<T> src) where T : IIdentityDetails => string.Join(',', src.ToEmails());
    public static string? ToFirstEmail<T>(this IEnumerable<T> src) where T : IIdentityDetails => src.ToEmails().FirstOrDefault();

    public static string? ToSingleEmail<T>(this IEnumerable<T> src) where T : IIdentityDetails
    {
        var emails = src.ToEmails();
        return emails.Count == 1 ? emails.First() : null;
    }
}
