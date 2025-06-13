using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationBillingDetails : ModelBase
{
    public string? CompanyName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string Suburb { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Province { get; set; }
    public string Zipcode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public Organization Organization { get; set; } = new();
}
