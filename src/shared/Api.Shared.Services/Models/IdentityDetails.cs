namespace Api.Shared.Services.Models;

public interface IIdentityDetails
{
    string? Email { get; set; }
    bool? EmailVerified { get; set; }
}

public static class IdentityDetailsExtensions
{
    extension(IEnumerable<string?> src)
    {
        public IReadOnlyList<string> ToEmails() =>
            src
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Select(item => item!.ToLowerInvariant())
                .Distinct()
                .ToList();
    }

    extension<T>(IEnumerable<T> src) where T : IIdentityDetails
    {
        public IReadOnlyList<string> ToEmails() => src.Select(item => item.Email).ToEmails();

        public string ToStringEmails() => string.Join(',', src.ToEmails());
        public string? ToFirstEmail() => src.ToEmails().FirstOrDefault();

        public string? ToSingleEmail()
        {
            var emails = src.ToEmails();
            return emails.Count == 1 ? emails.First() : null;
        }
    }
}
