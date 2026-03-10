namespace Api.Shared.Services.Models;

public record ListingMetadata(string About, string MainHeader, string SubHeader)
{
    public static ListingMetadata Empty() => new(string.Empty, string.Empty, string.Empty);
}
