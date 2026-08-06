using Api.Shared.Services.Offering;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("SpacesAccessErrorDetails")]
public sealed class SpacesAccessErrorDetails
{
    [GraphQLName("errorCode")]
    public string ErrorCode { get; set; } = string.Empty;

    [GraphQLName("status")]
    public SpacesSubscriptionStatus Status { get; set; }

    [GraphQLName("reasonCode")]
    public SpacesAccessReasonCode ReasonCode { get; set; }

    [GraphQLName("upgradeRequired")]
    public bool UpgradeRequired { get; set; }

    [GraphQLName("message")]
    public string Message { get; set; } = string.Empty;
}
