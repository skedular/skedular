using Api.Shared.Services.Models;
using Enterprise.Shared.Models;
using NetTopologySuite.Geometries;

namespace Location.Shared.Models;

public class LocationPhysicalAddress : ModelBaseWithDeleted, IAddressDetails
{
    public string? OsmType { get; set; }
    public string? OsmId { get; set; }
    public string? PlaceId { get; set; }
    public Point? Coordinates { get; set; }
    public Location Location { get; set; } = new();
    public string? CountryCode { get; set; }
    public string? FormattedAddress { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string Suburb { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Province { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
