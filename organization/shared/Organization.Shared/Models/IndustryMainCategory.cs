using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class IndustryMainCategory : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<IndustrySubCategory> IndustrySubCategories { get; set; } = [];
}
