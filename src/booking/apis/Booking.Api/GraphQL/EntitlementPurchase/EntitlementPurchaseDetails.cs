using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.GraphQL.Entitlement;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.EntitlementPurchase;

[GraphQLName("EntitlementPurchaseDetails")]
public class EntitlementPurchaseDetails
{
    public string Id { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string LifecycleState { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTimeOffset PaymentExpiry { get; set; }
    public DateTimeOffset ServiceStartAt { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PricingId { get; set; } = string.Empty;
    public int CreditQuantity { get; set; }
    public int ValidityDays { get; set; }
    public string CustomerId { get; set; } = string.Empty;

    public string OrganizationId { get; set; } = string.Empty;
    public string ProductVersionId { get; set; } = string.Empty;
    public string? EntitlementId { get; set; }
    public string? CheckoutReturnUrl { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceUrl { get; set; }
    public string? PaymentInstructions { get; set; }
    public string? PaymentAction { get; set; }
    public IReadOnlyList<string> InvoiceEmailList { get; set; } = [];

    public async Task<string?> CustomerNameAsync(
        [Service]
        ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetByIdAsync(CustomerId, cancellationToken);
        return customer?.ToDisplayableName();
    }

    public async Task<EntitlementDetails?> EntitlementAsync(
        [Service]
        IEntitlementReadService entitlementReadService,
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(EntitlementId))
        {
            return null;
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        EntitlementModel? entitlement;
        try
        {
            entitlement = await entitlementReadService.GetAuthorizedAsync(EntitlementId, customerId, cancellationToken);
        }
        catch (UnauthorizedAccessException)
        {
            entitlement = await entitlementReadService.GetAuthorizedForAdjustmentAsync(EntitlementId, customerId, cancellationToken);
        }

        return entitlement is null ? null : graphQlMapper.MapTo(entitlement);
    }

    public async Task<Connection<BookingEdge>> LinkedBookingsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        [Service]
        IEntitlementReadService entitlementReadService,
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IBookingService bookingService,
        [Service]
        IGraphQlMapper graphQlMapper,
        CancellationToken cancellationToken)
    {
        var entitlement = await GetAuthorizedEntitlementAsync(entitlementReadService, cachedCustomerService, cancellationToken);
        if (entitlement is null)
        {
            return new Connection<BookingEdge>
            {
                Edges = [],
                TotalCount = 0,
                PageInfo = new PageInfo
                {
                    HasPreviousPage = false,
                    HasNextPage = false,
                },
            };
        }

        var criteria = new BookingSearchCriteria(
            null, null, null, null, null, null, null, null, null, null, null, null, [], null, null,
            entitlement.OrganizationId, entitlement.OrganizationCustomDomain, [], [], [], [], EntitlementId);
        var (paginatedInfo, entries, totalCount) = await bookingService.GetPaginatedBookingsAsync(
            new PaginationInputParam(after, first, before, last),
            criteria,
            [new BookingOrder(OrderDirection.Ascending, BookingOrderField.From)],
            false,
            cancellationToken);

        return new Connection<BookingEdge>
        {
            Edges = entries.Select(item => new BookingEdge(graphQlMapper.MapTo(item.Node), item.Cursor)),
            TotalCount = totalCount,
            PageInfo = new PageInfo
            {
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                HasNextPage = paginatedInfo.HasNextPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
        };
    }

    private async Task<EntitlementModel?> GetAuthorizedEntitlementAsync(
        IEntitlementReadService entitlementReadService,
        ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(EntitlementId))
        {
            return null;
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        return await entitlementReadService.GetAuthorizedAsync(EntitlementId, customerId, cancellationToken);
    }
}

[ObjectType<EntitlementPurchaseDetails>]
public static partial class EntitlementPurchaseDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<EntitlementPurchaseDetails> descriptor) => descriptor.Ignore(item => item.ProductVersionId);

    public static ProductVersionDetails GetProductVersion([Parent] EntitlementPurchaseDetails item) => new(item.ProductVersionId);
}
