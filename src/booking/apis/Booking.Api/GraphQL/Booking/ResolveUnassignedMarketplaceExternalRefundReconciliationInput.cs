using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

public sealed class ResolveUnassignedMarketplaceExternalRefundReconciliationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("provider")] public required string Provider { get; set; }
    [GraphQLName("externalRefundId")] public required string ExternalRefundId { get; set; }
    [GraphQLName("status")] public required string Status { get; set; }
    [GraphQLName("reason")] public required string Reason { get; set; }
}
