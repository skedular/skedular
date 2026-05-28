using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class LocationRestrictedInformation : ModelBase
{
    public string Title { get; set; } = string.Empty;
    public LocationRestrictedInformationCategory Category { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
    public int SortOrder { get; set; }
    public Location Location { get; set; } = new();
}
