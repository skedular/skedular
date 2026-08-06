namespace Api.Shared.Services.Models;

public interface IAddressDetails
{
    string? FormattedAddress { get; set; }
    string AddressLine1 { get; set; }
    string? AddressLine2 { get; set; }
    string? Suburb { get; set; }
    string? City { get; set; }
    string? Province { get; set; }
    string Zipcode { get; set; }
    string Country { get; set; }
}

public static class AddressDetailsExtensions
{
    extension<T>(T src) where T : IAddressDetails
    {
        public string ToMultilinesFormattedAddress() =>
            new List<string?>
                {
                    src.AddressLine1,
                    src.AddressLine2,
                    src.Suburb,
                    src.City,
                    src.Province,
                    src.Zipcode,
                    src.Country,
                }.Where(item => !string.IsNullOrWhiteSpace(item))
                .Aggregate(string.Empty, (current, item) => $"{current}{Environment.NewLine}{item}").Trim();

        public string ToFormattedAddress() =>
            new List<string?>
                {
                    src.AddressLine1,
                    src.AddressLine2,
                    src.Suburb,
                    src.City,
                    src.Province,
                    src.Zipcode,
                    src.Country,
                }.Where(item => !string.IsNullOrWhiteSpace(item))
                .Aggregate(string.Empty, (current, item) => $"{current}, {item}").Trim().Trim(',');
    }
}
