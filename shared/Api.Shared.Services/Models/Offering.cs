using Api.Shared.Services.Offering;

namespace Api.Shared.Services.Models;

public class Offering
{
    public string Id { get; set; } = string.Empty;
    public OfferingCode Code { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public IReadOnlyList<string> ActiveCustomerIds { get; set; } = [];
}
