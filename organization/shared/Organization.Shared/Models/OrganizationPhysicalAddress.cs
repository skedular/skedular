using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationPhysicalAddress : ModelBaseWithDeleted, IAddressDetails
{
    public string FormattedAddress => this.ToFormattedAddress();
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public virtual Organization Organization { get; set; } = new();
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string Suburb { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Province { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
