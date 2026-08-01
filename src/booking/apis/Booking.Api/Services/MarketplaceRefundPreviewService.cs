using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;

namespace Booking.Api.Services;

public interface IMarketplaceRefundPreviewService
{
    Task<MarketplaceRefundPreviewModel> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken);
    Task<MarketplaceRefundPreviewModel> GetByMarketplaceBookingSubscriptionIdAsync(string subscriptionId, CancellationToken cancellationToken);
}

public class MarketplaceRefundPreviewService(
    IRepositoryFactory repositoryFactory,
    IMarketplaceRefundService marketplaceRefundService,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService) : IMarketplaceRefundPreviewService
{
    public async Task<MarketplaceRefundPreviewModel> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken)
    {
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken) ?? throw new BookingNotFound();
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);

        await EnsureAuthorizedAsync(ResolveOrganizationId(booking.MarketplaceBooking), cancellationToken);
        var result = MapTo(await marketplaceRefundService.GetBookingCancellationPreviewAsync(booking, cancellationToken));
        return result;
    }

    public async Task<MarketplaceRefundPreviewModel> GetByMarketplaceBookingSubscriptionIdAsync(
        string subscriptionId,
        CancellationToken cancellationToken)
    {
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken) ??
                           throw new MarketplaceBookingSubscriptionNotFound();
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking);

        await EnsureAuthorizedAsync(ResolveOrganizationId(subscription.MarketplaceBooking), cancellationToken);
        var result = MapTo(await marketplaceRefundService.GetImmediateSubscriptionCancellationPreviewAsync(subscription, cancellationToken));
        return result;
    }

    private async Task EnsureAuthorizedAsync(string organizationId, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        if (!await organizationAuthorizationService.CanModifyPaymentMethodAsync(organizationId, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }
    }

    private static string ResolveOrganizationId(MarketplaceBooking marketplaceBooking)
    {
        ArgumentNullException.ThrowIfNull(marketplaceBooking.ProductVersion);
        ArgumentNullException.ThrowIfNull(marketplaceBooking.ProductVersion.Product);
        ArgumentNullException.ThrowIfNull(marketplaceBooking.ProductVersion.Product.Organization);
        return marketplaceBooking.ProductVersion.Product.Organization.Id;
    }

    private static MarketplaceRefundPreviewModel MapTo(MarketplaceRefundPreview preview) => new()
    {
        LocalEntityType = preview.LocalEntityType.ToMarketplaceRefundEntityType(),
        LocalEntityId = preview.LocalEntityId,
        RequestedAt = preview.RequestedAt,
        ReferenceTime = preview.ReferenceTime,
        IsRefundable = preview.IsRefundable,
        RefundPercentage = preview.RefundPercentage,
        AppliedRuleMinutesBefore = preview.AppliedRuleMinutesBefore,
        BaseAmount = preview.BaseAmount,
        RefundAmount = preview.RefundAmount,
        Currency = preview.Currency.ToNullableCurrency()
    };
}
