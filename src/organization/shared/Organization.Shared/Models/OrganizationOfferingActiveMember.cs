using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationOfferingActiveMember : ModelBase
{
    public OrganizationMember OrganizationMember { get; set; } = new();
    public OrganizationOffering OrganizationOffering { get; set; } = new();
}
