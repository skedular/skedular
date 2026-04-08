using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Random;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.Services;

public interface IMarketplaceRefundService
{
    Task<MarketplaceRefundPreview> GetBookingCancellationPreviewAsync(
        BookingEntity booking,
        CancellationToken cancellationToken);

    Task<MarketplaceRefundPreview> GetImmediateSubscriptionCancellationPreviewAsync(
        MarketplaceBookingSubscriptionEntity subscription,
        CancellationToken cancellationToken);

    Task<MarketplaceRefund?> CreateBookingCancellationRefundAsync(
        BookingEntity booking,
        CustomerEntity? requestedByCustomer,
        CancellationToken cancellationToken);

    Task<MarketplaceRefund?> CreateImmediateSubscriptionCancellationRefundAsync(
        MarketplaceBookingSubscriptionEntity subscription,
        CustomerEntity? requestedByCustomer,
        CancellationToken cancellationToken);

    Task<bool> HasConfirmedPaymentAsync(
        MarketplaceRefund refund,
        CancellationToken cancellationToken);
}

public class MarketplaceRefundService(
    IRepositoryFactory repositoryFactory,
    MarketplaceRefundPolicyService marketplaceRefundPolicyService,
    IMarketplaceRefundEventService marketplaceRefundEventService,
    IRandomHelper randomHelper,
    TimeProvider timeProvider) : IMarketplaceRefundService
{
    public Task<MarketplaceRefundPreview> GetBookingCancellationPreviewAsync(
        BookingEntity booking,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);
        var organizationId = ResolveOrganizationId(booking);
        var hasConfirmedPayment = HasConfirmedPayment(booking.MarketplaceBooking);
        return GetPreviewAsync(
            organizationId,
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            booking.MarketplaceBooking.Id,
            booking.MarketplaceBooking,
            booking.From,
            hasConfirmedPayment,
            cancellationToken);
    }

    public Task<MarketplaceRefundPreview> GetImmediateSubscriptionCancellationPreviewAsync(
        MarketplaceBookingSubscriptionEntity subscription,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking);
        var organizationId = ResolveOrganizationId(subscription);
        var requestedAt = timeProvider.GetUtcNow();
        var referenceTime = subscription.NextRenewalAt ?? subscription.StartedAt;
        var recurringBooking = ResolveCurrentBillingWindowRecurringBooking(subscription, requestedAt);
        var hasConfirmedPayment = HasConfirmedPayment(recurringBooking?.MarketplaceBooking);
        return GetPreviewAsync(
            organizationId,
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            subscription.Id,
            subscription.MarketplaceBooking,
            referenceTime,
            hasConfirmedPayment,
            cancellationToken);
    }

    public async Task<MarketplaceRefund?> CreateBookingCancellationRefundAsync(
        BookingEntity booking,
        CustomerEntity? requestedByCustomer,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);
        var organizationId = ResolveOrganizationId(booking);
        return await UpsertRefundAsync(
            organizationId,
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            booking.MarketplaceBooking.Id,
            booking.MarketplaceBooking,
            booking.From,
            requestedByCustomer,
            cancellationToken);
    }

    public async Task<MarketplaceRefund?> CreateImmediateSubscriptionCancellationRefundAsync(
        MarketplaceBookingSubscriptionEntity subscription,
        CustomerEntity? requestedByCustomer,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking);
        var organizationId = ResolveOrganizationId(subscription);
        var referenceTime = subscription.NextRenewalAt ?? subscription.StartedAt;
        return await UpsertRefundAsync(
            organizationId,
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            subscription.Id,
            subscription.MarketplaceBooking,
            referenceTime,
            requestedByCustomer,
            cancellationToken);
    }

    public async Task<bool> HasConfirmedPaymentAsync(
        MarketplaceRefund refund,
        CancellationToken cancellationToken)
    {
        switch (refund.LocalEntityType)
        {
            case MarketplaceRefundEntityTypeConstants.MarketplaceBooking:
                {
                    var marketplaceBooking =
                        await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(refund.LocalEntityId, cancellationToken);
                    return HasConfirmedPayment(marketplaceBooking);
                }

            case MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription:
                {
                    var subscription =
                        await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(refund.LocalEntityId, cancellationToken);
                    if (subscription is null)
                    {
                        return false;
                    }

                    var recurringBooking = ResolveCurrentBillingWindowRecurringBooking(subscription, refund.RequestedAt);
                    return HasConfirmedPayment(recurringBooking?.MarketplaceBooking);
                }

            default:
                return false;
        }
    }

    private async Task<MarketplaceRefund?> UpsertRefundAsync(
        string organizationId,
        string localEntityType,
        string localEntityId,
        MarketplaceBookingEntity marketplaceBooking,
        DateTimeOffset referenceTime,
        CustomerEntity? requestedByCustomer,
        CancellationToken cancellationToken)
    {
        var hasConfirmedPayment = localEntityType switch
        {
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking => HasConfirmedPayment(marketplaceBooking),
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription => await HasConfirmedPaymentAsync(
                new MarketplaceRefund { LocalEntityType = localEntityType, LocalEntityId = localEntityId, RequestedAt = timeProvider.GetUtcNow() },
                cancellationToken),
            _ => false
        };

        var preview = await GetPreviewAsync(
            organizationId,
            localEntityType,
            localEntityId,
            marketplaceBooking,
            referenceTime,
            hasConfirmedPayment,
            cancellationToken);
        if (!preview.IsRefundable)
        {
            return null;
        }

        var existingRefund = await repositoryFactory.MarketplaceRefundRepository.GetByLocalEntityAsync(
            organizationId,
            localEntityType,
            localEntityId,
            cancellationToken);

        if (existingRefund is null)
        {
            var refund = repositoryFactory.MarketplaceRefundRepository.Add(
                new MarketplaceRefund
                {
                    Id = randomHelper.Generate(),
                    OrganizationId = organizationId,
                    LocalEntityType = localEntityType,
                    LocalEntityId = localEntityId,
                    Status = MarketplaceRefundStatusConstants.Requested,
                    RequestedAt = preview.RequestedAt,
                    ReferenceTime = preview.ReferenceTime,
                    RefundPercentage = preview.RefundPercentage,
                    AppliedRuleMinutesBefore = preview.AppliedRuleMinutesBefore,
                    BaseAmount = preview.BaseAmount,
                    RefundAmount = preview.RefundAmount,
                    Currency = ResolveRefundCurrency(marketplaceBooking, preview),
                    RequestedByCustomer = requestedByCustomer
                });
            marketplaceRefundEventService.Add(
                refund,
                MarketplaceRefundEventTypeConstants.Requested,
                requestedByCustomer?.Id,
                refund.RequestedAt);
            return refund;
        }

        existingRefund.Status = MarketplaceRefundStatusConstants.Requested;
        existingRefund.RequestedAt = preview.RequestedAt;
        existingRefund.ReferenceTime = preview.ReferenceTime;
        existingRefund.RefundPercentage = preview.RefundPercentage;
        existingRefund.AppliedRuleMinutesBefore = preview.AppliedRuleMinutesBefore;
        existingRefund.BaseAmount = preview.BaseAmount;
        existingRefund.RefundAmount = preview.RefundAmount;
        existingRefund.Currency = ResolveRefundCurrency(marketplaceBooking, preview);
        existingRefund.RequestedByCustomer = requestedByCustomer;
        var updatedRefund = repositoryFactory.MarketplaceRefundRepository.Update(existingRefund);
        marketplaceRefundEventService.Add(
            updatedRefund,
            MarketplaceRefundEventTypeConstants.Requested,
            requestedByCustomer?.Id,
            updatedRefund.RequestedAt);
        return updatedRefund;
    }

    private Task<MarketplaceRefundPreview> GetPreviewAsync(
        string organizationId,
        string localEntityType,
        string localEntityId,
        MarketplaceBookingEntity marketplaceBooking,
        DateTimeOffset referenceTime,
        bool hasConfirmedPayment,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var requestedAt = timeProvider.GetUtcNow();
        if (!hasConfirmedPayment)
        {
            return Task.FromResult(
                new MarketplaceRefundPreview(
                    organizationId,
                    localEntityType,
                    localEntityId,
                    requestedAt,
                    referenceTime,
                    false,
                    0,
                    null,
                    null,
                    null,
                    marketplaceBooking.Currency));
        }

        var quote = marketplaceRefundPolicyService.GetQuote(marketplaceBooking.ProductPricing, referenceTime, requestedAt);
        var baseAmount = ResolveBaseAmount(marketplaceBooking);
        decimal? refundAmount = quote.IsRefundable && baseAmount is not null
            ? quote.CalculateRefundAmount(baseAmount.Value)
            : null;

        return Task.FromResult(
            new MarketplaceRefundPreview(
                organizationId,
                localEntityType,
                localEntityId,
                requestedAt,
                referenceTime,
                quote.IsRefundable,
                quote.RefundPercentage,
                quote.AppliedRuleMinutesBefore,
                baseAmount,
                refundAmount,
                marketplaceBooking.Currency));
    }

    private static string? ResolveRefundCurrency(MarketplaceBookingEntity marketplaceBooking, MarketplaceRefundPreview preview) =>
        preview.Currency ?? marketplaceBooking.Currency ?? marketplaceBooking.ProductVersion?.Currency;

    private static decimal? ResolveBaseAmount(MarketplaceBookingEntity marketplaceBooking)
    {
        if (marketplaceBooking.TotalAmount.HasValue)
        {
            return marketplaceBooking.TotalAmount.Value.RoundedDecimal();
        }

        if (marketplaceBooking.TotalAmountExcludeTax.HasValue && marketplaceBooking.TaxAmount.HasValue)
        {
            return (marketplaceBooking.TotalAmountExcludeTax.Value + marketplaceBooking.TaxAmount.Value).RoundedDecimal();
        }

        return marketplaceBooking.ProductPricing.IsTaxInclusive
            ? (marketplaceBooking.ProductPricing.Price * Math.Max(marketplaceBooking.Quantity, 1)).RoundedDecimal()
            : marketplaceBooking.TotalAmountExcludeTax?.RoundedDecimal();
    }

    private static string ResolveOrganizationId(BookingEntity booking)
    {
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking.ProductVersion);
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking.ProductVersion.Product);
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking.ProductVersion.Product.Organization);
        return booking.MarketplaceBooking.ProductVersion.Product.Organization.Id;
    }

    private static string ResolveOrganizationId(MarketplaceBookingSubscriptionEntity subscription)
    {
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking);
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking.ProductVersion);
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking.ProductVersion.Product);
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking.ProductVersion.Product.Organization);
        return subscription.MarketplaceBooking.ProductVersion.Product.Organization.Id;
    }

    private static bool HasConfirmedPayment(MarketplaceBookingEntity? marketplaceBooking) =>
        marketplaceBooking?.PaymentStatus == PaymentStatus.Confirmed.ToPaymentStatus();

    private static RecurringBookingEntity? ResolveCurrentBillingWindowRecurringBooking(
        MarketplaceBookingSubscriptionEntity subscription,
        DateTimeOffset now)
    {
        var recurringBookingsInWindow = subscription.RecurringBookings
            .Where(item => !item.DeletedAt.HasValue && item.MarketplaceBooking is not null)
            .Where(item => IntersectsBillingWindow(subscription, item, now))
            .OrderBy(item => item.StartDate)
            .ToList();

        return recurringBookingsInWindow.LastOrDefault();
    }

    private static bool IntersectsBillingWindow(
        MarketplaceBookingSubscriptionEntity subscription,
        RecurringBookingEntity recurringBooking,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking);
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking.ProductVersion);
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking.ProductVersion.Product);
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking.ProductVersion.Product.Organization);

        var (windowStartInclusive, windowEndExclusive) = ResolveCurrentBillingWindow(
            subscription.StartedAt,
            now,
            subscription.MarketplaceBooking.ProductVersion.Product.Organization.BillingCycle);
        var recurringBookingEndExclusive = recurringBooking.EndDate?.AddDays(1) ??
                                           ResolveRecurringBookingCycleEndExclusive(recurringBooking);

        return recurringBooking.StartDate < windowEndExclusive && recurringBookingEndExclusive > windowStartInclusive;
    }

    private static (DateTimeOffset StartInclusive, DateTimeOffset EndExclusive) ResolveCurrentBillingWindow(
        DateTimeOffset startedAt,
        DateTimeOffset now,
        string organizationBillingCycle)
    {
        var startInclusive = startedAt;
        var endExclusive = AdvanceBillingWindow(startInclusive, organizationBillingCycle);

        while (now >= endExclusive)
        {
            startInclusive = endExclusive;
            endExclusive = AdvanceBillingWindow(startInclusive, organizationBillingCycle);
        }

        return (startInclusive, endExclusive);
    }

    private static DateTimeOffset AdvanceBillingWindow(DateTimeOffset startInclusive, string organizationBillingCycle) =>
        organizationBillingCycle switch
        {
            OrganizationBillingCycleConstants.Weekly => startInclusive.AddDays(7),
            OrganizationBillingCycleConstants.Fortnightly => startInclusive.AddDays(14),
            OrganizationBillingCycleConstants.Monthly => startInclusive.AddMonths(1),
            _ => throw new ArgumentOutOfRangeException(nameof(organizationBillingCycle))
        };

    private static DateTimeOffset ResolveRecurringBookingCycleEndExclusive(RecurringBookingEntity recurringBooking)
    {
        ArgumentNullException.ThrowIfNull(recurringBooking.MarketplaceBooking);

        return recurringBooking.MarketplaceBooking.ProductPricing.PurchaseCadence switch
        {
            ProductPricingCadence.Weekly => recurringBooking.StartDate.AddDays(7),
            ProductPricingCadence.Fortnightly => recurringBooking.StartDate.AddDays(14),
            ProductPricingCadence.Monthly => recurringBooking.StartDate.AddMonths(1),
            ProductPricingCadence.TwoMonths => recurringBooking.StartDate.AddMonths(2),
            ProductPricingCadence.Quarterly => recurringBooking.StartDate.AddMonths(3),
            ProductPricingCadence.FourMonths => recurringBooking.StartDate.AddMonths(4),
            ProductPricingCadence.FiveMonths => recurringBooking.StartDate.AddMonths(5),
            ProductPricingCadence.SixMonths => recurringBooking.StartDate.AddMonths(6),
            ProductPricingCadence.Yearly => recurringBooking.StartDate.AddYears(1),
            _ => recurringBooking.StartDate.AddDays(1)
        };
    }
}
