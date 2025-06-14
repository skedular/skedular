using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class CustomerBillingDetails : ModelBase
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
    public Customer Customer { get; set; } = new();
}
