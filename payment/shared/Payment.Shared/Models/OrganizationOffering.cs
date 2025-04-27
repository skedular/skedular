using Api.Shared.Services.Offering;
using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class OrganizationOffering : ReplicatedModelBaseWithDeleted
{
    public OfferingCode Code { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public Organization Organization { get; set; } = new();
    public StripePaymentIntent? StripePaymentIntent { get; set; }
}
