using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationTaxDetails : ModelBase
{
    public bool IsRegistered { get; set; } = true;
    public string TaxId { get; set; } = string.Empty;
    public decimal TaxRatePercentage { get; set; }
    public Organization Organization { get; set; } = new();
}
