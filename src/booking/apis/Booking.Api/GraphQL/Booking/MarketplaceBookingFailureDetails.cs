using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("MarketplaceBookingFailureChoiceDetails")]
public class MarketplaceBookingFailureChoiceDetails
{
    [GraphQLName("type")]
    public string Type { get; set; } = string.Empty;

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}

[GraphQLName("MarketplaceBookingFailureDeliveryDetails")]
public class MarketplaceBookingFailureDeliveryDetails
{
    [GraphQLName("audience")]
    public MarketplaceBookingFailureChoiceDetails Audience { get; set; } = new();

    [GraphQLName("channel")]
    public MarketplaceBookingFailureChoiceDetails Channel { get; set; } = new();

    [GraphQLName("status")]
    public MarketplaceBookingFailureChoiceDetails Status { get; set; } = new();

    [GraphQLName("sentAt")]
    public DateTimeOffset? SentAt { get; set; }
}

[GraphQLName("MarketplaceBookingFailureDetails")]
public class MarketplaceBookingFailureDetails
{
    [GraphQLName("id")]
    public string Id { get; set; } = string.Empty;

    [GraphQLName("category")]
    public MarketplaceBookingFailureChoiceDetails Category { get; set; } = new();

    [GraphQLName("scope")]
    public MarketplaceBookingFailureChoiceDetails Scope { get; set; } = new();

    [GraphQLName("finalizedAt")]
    public DateTimeOffset FinalizedAt { get; set; }

    [GraphQLName("requestedFrom")]
    public DateTimeOffset? RequestedFrom { get; set; }

    [GraphQLName("requestedUntil")]
    public DateTimeOffset? RequestedUntil { get; set; }

    [GraphQLName("customerAction")]
    public MarketplaceBookingFailureChoiceDetails CustomerAction { get; set; } = new();

    [GraphQLName("resolutionDeadlineAt")]
    public DateTimeOffset? ResolutionDeadlineAt { get; set; }

    [GraphQLName("resolutionDecidedAt")]
    public DateTimeOffset? ResolutionDecidedAt { get; set; }

    [GraphQLName("resolutionDecision")]
    public string? ResolutionDecision { get; set; }

    [GraphQLName("allocatedRefundAmount")]
    public decimal? AllocatedRefundAmount { get; set; }

    [GraphQLName("resourceReleaseStatus")]
    public MarketplaceBookingFailureResourceReleaseStatusDetails ResourceReleaseStatus { get; set; } = new();

    [GraphQLName("accountingCleanupStatus")]
    public MarketplaceBookingFailureAccountingCleanupStatusDetails AccountingCleanupStatus { get; set; } = new();
}

[GraphQLName("MarketplaceBookingFailureResourceReleaseStatusDetails")]
public class MarketplaceBookingFailureResourceReleaseStatusDetails
{
    public MarketplaceBookingFailureResourceReleaseStatus Type { get; set; }
    public string Name { get; set; } = string.Empty;
}

[GraphQLName("MarketplaceBookingFailureAccountingCleanupStatusDetails")]
public class MarketplaceBookingFailureAccountingCleanupStatusDetails
{
    public MarketplaceBookingFailureAccountingCleanupStatus Type { get; set; }
    public string Name { get; set; } = string.Empty;
}
