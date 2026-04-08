using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;

namespace Booking.Api.Services;

public interface IMarketplaceRefundPreviewService
{
    Task<MarketplaceRefundPreviewDetails> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken);
    Task<MarketplaceRefundPreviewDetails> GetByMarketplaceBookingSubscriptionIdAsync(string subscriptionId, CancellationToken cancellationToken);
}

public class MarketplaceRefundPreviewService(
    IRepositoryFactory repositoryFactory,
    IMarketplaceRefundService marketplaceRefundService,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService) : IMarketplaceRefundPreviewService
{
    public async Task<MarketplaceRefundPreviewDetails> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken)
    {
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken) ?? throw new BookingNotFound();
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);

        await EnsureAuthorizedAsync(ResolveOrganizationId(booking.MarketplaceBooking), cancellationToken);
        return MapTo(await marketplaceRefundService.GetBookingCancellationPreviewAsync(booking, cancellationToken));
    }

    public async Task<MarketplaceRefundPreviewDetails> GetByMarketplaceBookingSubscriptionIdAsync(
        string subscriptionId,
        CancellationToken cancellationToken)
    {
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(subscriptionId, cancellationToken) ??
                           throw new MarketplaceBookingSubscriptionNotFound();
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking);

        await EnsureAuthorizedAsync(ResolveOrganizationId(subscription.MarketplaceBooking), cancellationToken);
        return MapTo(await marketplaceRefundService.GetImmediateSubscriptionCancellationPreviewAsync(subscription, cancellationToken));
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

    private static MarketplaceRefundPreviewDetails MapTo(MarketplaceRefundPreview preview)
    {
        var currency = preview.Currency.ToNullableCurrency();

        return new MarketplaceRefundPreviewDetails
        {
            LocalEntityType = preview.LocalEntityType,
            LocalEntityId = preview.LocalEntityId,
            RequestedAt = preview.RequestedAt,
            ReferenceTime = preview.ReferenceTime,
            IsRefundable = preview.IsRefundable,
            RefundPercentage = preview.RefundPercentage,
            AppliedRuleMinutesBefore = preview.AppliedRuleMinutesBefore,
            BaseAmount = preview.BaseAmount,
            RefundAmount = preview.RefundAmount,
            Currency = currency is null ? null : new CurrencyDetails { Type = currency.Value, Name = currency.Value.ToCurrencyName() },
            CurrencyToDisplay = currency is null ? "N/A" : currency.Value.ToCurrencyName()
        };
    }
}
