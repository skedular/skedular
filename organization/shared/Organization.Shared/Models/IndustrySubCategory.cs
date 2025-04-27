using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class IndustrySubCategory : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public IndustryMainCategory IndustryMainCategory { get; set; } = new();
    public ICollection<Organization> Organizations { get; set; } = [];
}
