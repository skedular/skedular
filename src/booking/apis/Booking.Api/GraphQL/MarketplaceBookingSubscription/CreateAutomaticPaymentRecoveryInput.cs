using HotChocolate;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

[GraphQLName("CreateMarketplaceBookingSubscriptionAutomaticPaymentRecoveryInput")]
public sealed class CreateAutomaticPaymentRecoveryInput
{
    public string ClientMutationId { get; set; } = string.Empty;
    public string SubscriptionId { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}
