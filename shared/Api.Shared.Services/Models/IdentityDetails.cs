namespace Api.Shared.Services.Models;

public interface IIdentityDetails
{
    string? Email { get; set; }
    bool? EmailVerified { get; set; }
}

public static class IdentityDetailsExtensions
{
    public static string? ToSingleEmail<T>(this ICollection<T> src) where T : IIdentityDetails =>
        src.Where(identity => !string.IsNullOrWhiteSpace(identity.Email))
            .Select(item => item.Email!.ToLowerInvariant())
            .Distinct()
            .FirstOrDefault();
}
