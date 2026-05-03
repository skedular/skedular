namespace Api.Shared.Services.Models;

public record ListingMetadata(string? About, string? Title, string? SubTitle, IReadOnlyList<string>? IncludedFeatures)
{
    public static ListingMetadata Empty => new(string.Empty, string.Empty, string.Empty, []);
}
