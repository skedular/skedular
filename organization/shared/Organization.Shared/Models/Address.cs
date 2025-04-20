using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class Address : ModelBase
{
    public string? FormattedAddress { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? Suburb { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public string? Zipcode { get; set; }
    public string? Country { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public Organization Organization { get; set; }
}
