using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class Address : ModelBase, IAddressDetails
{
    public string FormattedAddress => this.ToFormattedAddress();
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public Organization? Organization { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string Suburb { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Province { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
