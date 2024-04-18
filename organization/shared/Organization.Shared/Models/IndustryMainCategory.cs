using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class IndustryMainCategory : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public ICollection<IndustrySubCategory> IndustrySubCategories { get; set; } = [];
}
