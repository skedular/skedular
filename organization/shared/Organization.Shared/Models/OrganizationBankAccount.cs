using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationBankAccount : ModelBaseWithDeleted
{
    public bool IsDefault { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public Organization Organization { get; set; } = new();
}
