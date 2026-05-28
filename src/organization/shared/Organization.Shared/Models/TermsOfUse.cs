using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class TermsOfUse : ModelBaseWithDeleted
{
    public bool Active { get; set; }
    public string Terms { get; set; } = string.Empty;
    public IReadOnlyList<Organization> Organizations { get; set; } = [];
}
