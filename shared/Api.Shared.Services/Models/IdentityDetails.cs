namespace Api.Shared.Services.Models;

public interface IIdentityDetails
{
    string? Email { get; set; }
    bool? EmailVerified { get; set; }
}

public static class IdentityDetailsExtensions
{
    public static ICollection<string> ToEmails<T>(this ICollection<T> src) where T : IIdentityDetails =>
        src.Where(identity => !string.IsNullOrWhiteSpace(identity.Email))
            .Select(item => item.Email!.ToLowerInvariant())
            .Distinct()
            .ToList();

    public static string? ToFirstEmail<T>(this ICollection<T> src) where T : IIdentityDetails =>
        src.ToEmails().FirstOrDefault();

    public static string? ToSingleEmail<T>(this ICollection<T> src) where T : IIdentityDetails
    {
        var emails = src.ToEmails();
        return emails.Count == 1 ? emails.First() : null;
    }
}
