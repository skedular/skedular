using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationTaxDetails : ModelBase
{
    public string GstNumber { get; set; } = string.Empty;
    public decimal GstPercentage { get; set; }
    public Organization Organization { get; set; } = new();
}
