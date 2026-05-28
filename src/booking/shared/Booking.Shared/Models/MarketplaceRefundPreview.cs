namespace Booking.Shared.Models;

public record MarketplaceRefundPreview(
    string OrganizationId,
    string LocalEntityType,
    string LocalEntityId,
    DateTimeOffset RequestedAt,
    DateTimeOffset ReferenceTime,
    bool IsRefundable,
    int RefundPercentage,
    int? AppliedRuleMinutesBefore,
    decimal? BaseAmount,
    decimal? RefundAmount,
    string? Currency);
