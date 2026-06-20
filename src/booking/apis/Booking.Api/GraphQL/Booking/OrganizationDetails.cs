using Booking.Shared.Services;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("OrganizationDetails")]
[EntityKey("id")]
[Shareable]
public class OrganizationDetails(string id, string customDomain) : Node(id)
{
    [GraphQLName("customDomain")] public string? CustomDomain { get; set; } = customDomain;
}

[ObjectType<OrganizationDetails>]
public static partial class OrganizationDetailsType
{
    public static async Task<SpacesPublicBookingAvailabilityDetails> GetSpacesPublicBookingAvailabilityAsync(
        [Parent] OrganizationDetails organization,
        [Service] ISpacesBookingQuotaService spacesBookingQuotaService,
        CancellationToken cancellationToken)
    {
        var decision = await spacesBookingQuotaService.GetQuotaStatusAsync(organization.Id, cancellationToken);
        return new SpacesPublicBookingAvailabilityDetails
        {
            Available = decision.CanCreate,
            Message = decision.CanCreate
                ? "Bookings are available."
                : "Bookings are currently unavailable for this workspace."
        };
    }
}

[GraphQLName("SpacesPublicBookingAvailabilityDetails")]
public sealed class SpacesPublicBookingAvailabilityDetails
{
    [GraphQLName("available")] public bool Available { get; set; }
    [GraphQLName("message")] public string Message { get; set; } = string.Empty;
}
