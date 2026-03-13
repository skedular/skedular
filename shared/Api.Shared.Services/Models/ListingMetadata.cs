namespace Api.Shared.Services.Models;

public record ListingMetadata(string? About, string? Title, string? SubTitle, ICollection<string>? IncludedFeatures)
{
    public static ListingMetadata Empty => new(string.Empty, string.Empty, string.Empty, []);
}
