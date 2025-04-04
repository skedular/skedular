using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Product : ModelBaseWithDeleted
{
    public bool Inactive { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public PriceUnit PriceUnit { get; set; }
    public decimal PricePerMinute { get; set; }
    public Currency Currency { get; set; }
    public int? MinDurationMinutes { get; set; }
    public int? MaxDurationMinutes { get; set; }
    public bool BookAllLocationResources { get; set; }
    public int RecurrenceIntervalDays { get; set; }
    public bool ForceContinuousSlots { get; set; }
    public int? MaxSpreadDays { get; set; }
    public Organization Organization { get; set; }
    public ICollection<OrganizationTag> ProductTags { get; set; } = [];
    public ICollection<OrganizationTag> LocationTags { get; set; } = [];
    public ICollection<ProductVersion> ProductVersions { get; set; } = [];
}
