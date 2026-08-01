using System.Text.Json;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;
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
        CancellationToken cancellationToken,
        bool forceFullRefund = false);

    Task<MarketplaceRefund?> CreateImmediateSubscriptionCancellationRefundAsync(
        MarketplaceBookingSubscriptionEntity subscription,
        CustomerEntity? requestedByCustomer,
        CancellationToken cancellationToken,
        bool forceFullRefund = false);

    Task<MarketplaceRefund?> CreateModificationRefundAsync(
        BookingEntity booking,
        decimal originalAmount,
        decimal newAmount,
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
    ITemporalOutboxService temporalOutboxService,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    ILogger<MarketplaceRefundService> logger) : IMarketplaceRefundService
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
        var recurringBooking = ResolveCurrentBillingWindowRecurringBooking(subscription, requestedAt);
        var hasConfirmedPayment = HasConfirmedPayment(recurringBooking?.MarketplaceBooking);
        return Task.FromResult(GetSubscriptionProRatedPreview(
            organizationId,
            subscription,
            recurringBooking,
            requestedAt,
            hasConfirmedPayment));
    }

    public async Task<MarketplaceRefund?> CreateBookingCancellationRefundAsync(
        BookingEntity booking,
        CustomerEntity? requestedByCustomer,
        CancellationToken cancellationToken,
        bool forceFullRefund = false)
    {
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);
        var organizationId = ResolveOrganizationId(booking);
        return await UpsertRefundAsync(
            organizationId,
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            booking.MarketplaceBooking.Id,
            booking.MarketplaceBooking,
            booking.From,
            ResolveTimezoneId(booking.InvolvedLocations.Select(item => item.Timezone)),
            requestedByCustomer,
            cancellationToken,
            forceFullRefund);
    }

    public async Task<MarketplaceRefund?> CreateImmediateSubscriptionCancellationRefundAsync(
        MarketplaceBookingSubscriptionEntity subscription,
        CustomerEntity? requestedByCustomer,
        CancellationToken cancellationToken,
        bool forceFullRefund = false)
    {
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking);
        var organizationId = ResolveOrganizationId(subscription);
        var referenceTime = subscription.NextRenewalAt ?? subscription.StartedAt;
        var requestedAt = timeProvider.GetUtcNow();
        var recurringBooking = ResolveCurrentBillingWindowRecurringBooking(subscription, requestedAt);
        var hasConfirmedPayment = HasConfirmedPayment(recurringBooking?.MarketplaceBooking);
        var proRatedPreview = GetSubscriptionProRatedPreview(
            organizationId,
            subscription,
            recurringBooking,
            requestedAt,
            hasConfirmedPayment);
        // The subscription marketplace booking is a pricing template. Its payment fields are
        // intentionally unset, while the current recurring booking carries the confirmed bank
        // transfer or card payment that can actually be refunded.
        var refundSourceMarketplaceBooking = recurringBooking?.MarketplaceBooking ?? subscription.MarketplaceBooking;
        return await UpsertRefundAsync(
            organizationId,
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            subscription.Id,
            refundSourceMarketplaceBooking,
            referenceTime,
            ResolveTimezoneId(subscription.RequestedResources.Select(item => item.Location?.Timezone)),
            requestedByCustomer,
            cancellationToken,
            forceFullRefund,
            proRatedPreview);
    }

    public async Task<MarketplaceRefund?> CreateModificationRefundAsync(
        BookingEntity booking, decimal originalAmount, decimal newAmount, CustomerEntity? requestedByCustomer, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);
        if (newAmount >= originalAmount)
        {
            return null;
        }

        var hasPayment = HasConfirmedPayment(booking.MarketplaceBooking);
        if (!hasPayment)
        {
            return null;
        }

        var amount = originalAmount - newAmount;
        var organizationId = ResolveOrganizationId(booking);
        var idempotencyKey =
            $"modification:{MarketplaceRefundEntityTypeConstants.MarketplaceBooking}:{booking.MarketplaceBooking.Id}:{originalAmount:0.####}:{newAmount:0.####}";
        var existing = await repositoryFactory.MarketplaceRefundRepository.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);
        if (existing is not null)
        {
            return existing;
        }

        var refund = repositoryFactory.MarketplaceRefundRepository.Add(new MarketplaceRefund
        {
            Id = randomHelper.Generate(),
            IdempotencyKey = idempotencyKey,
            OrganizationId = organizationId,
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = booking.MarketplaceBooking.Id,
            RefundKind = MarketplaceRefundKindConstants.Modification,
            Status = ResolveInitialStatus(booking.MarketplaceBooking),
            RequestedAt = timeProvider.GetUtcNow(),
            ReferenceTime = booking.From,
            BaseAmount = originalAmount,
            RefundAmount = amount,
            RefundPercentage = originalAmount == 0 ? 0 : (int)Math.Round(amount / originalAmount * 100),
            Currency = booking.MarketplaceBooking.Currency,
            TimezoneId = ResolveTimezoneId(booking.InvolvedLocations.Select(item => item.Timezone)),
            RequestedByCustomerId = requestedByCustomer?.Id
        });
        await AddPaymentAllocationIfAvailableAsync(refund, booking.MarketplaceBooking, cancellationToken);
        marketplaceRefundEventService.Add(refund, MapInitialEventType(refund.Status), requestedByCustomer?.Id, refund.RequestedAt);
        if (refund.Status == MarketplaceRefundStatusConstants.Requested)
        {
            temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                new ProcessMarketplaceRefundInput(refund.Id, requestedByCustomer?.Id), repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return refund;
    }

    public async Task<bool> HasConfirmedPaymentAsync(
        MarketplaceRefund refund,
        CancellationToken cancellationToken)
    {
        // The refund allocation is a durable snapshot of the payment that funded this
        // refund. It must remain authoritative after subscription cancellation, because
        // cancellation can change the active recurring booking projection or remove
        // future instances without undoing a payment that was already confirmed.
        if (refund.PaymentAllocations.Any(item => item.IsSourcePayment && item.SourceCapturedAmount > 0m))
        {
            return true;
        }

        foreach (var sourceReference in refund.PaymentAllocations
                     .Where(item => item.IsSourcePayment && !string.IsNullOrWhiteSpace(item.SourcePaymentReference))
                     .Select(item => item.SourcePaymentReference)
                     .Distinct())
        {
            var sourceBooking = await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(sourceReference, cancellationToken);
            if (HasConfirmedPayment(sourceBooking))
            {
                return true;
            }
        }

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
                    if (HasConfirmedPayment(recurringBooking?.MarketplaceBooking))
                    {
                        return true;
                    }

                    // Cancellation can end the current recurring projection before an
                    // operator approves the refund. Preserve eligibility when another
                    // generated cycle already records the confirmed payment.
                    return subscription.RecurringBookings.Any(item => HasConfirmedPayment(item.MarketplaceBooking));
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
        string timezoneId,
        CustomerEntity? requestedByCustomer,
        CancellationToken cancellationToken,
        bool forceFullRefund = false,
        MarketplaceRefundPreview? overridePreview = null)
    {
        var hasConfirmedPayment = localEntityType switch
        {
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking => HasConfirmedPayment(marketplaceBooking),
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription => await HasConfirmedPaymentAsync(
                new MarketplaceRefund { LocalEntityType = localEntityType, LocalEntityId = localEntityId, RequestedAt = timeProvider.GetUtcNow() },
                cancellationToken),
            _ => false
        };

        var preview = overridePreview ?? await GetPreviewAsync(
            organizationId,
            localEntityType,
            localEntityId,
            marketplaceBooking,
            referenceTime,
            hasConfirmedPayment,
            cancellationToken);

        // Operator-initiated cancellation always produces a full refund regardless of policy
        if (forceFullRefund && hasConfirmedPayment)
        {
            var baseAmount = ResolveBaseAmount(marketplaceBooking);
            preview = preview with
            {
                IsRefundable = true,
                RefundPercentage = 100,
                AppliedRuleMinutesBefore = null,
                BaseAmount = baseAmount,
                RefundAmount = baseAmount
            };
        }

        if (!preview.IsRefundable)
        {
            return null;
        }

        var idempotencyKey = $"cancellation:{localEntityType}:{localEntityId}";
        var existingRefund = await repositoryFactory.MarketplaceRefundRepository.GetByIdempotencyKeyAsync(idempotencyKey, cancellationToken);

        if (existingRefund is null)
        {
            var policySnapshot = BuildPolicySnapshot(marketplaceBooking.ProductPricing, marketplaceBooking.CreatedAt);
            var refund = repositoryFactory.MarketplaceRefundRepository.Add(
                new MarketplaceRefund
                {
                    Id = randomHelper.Generate(),
                    IdempotencyKey = idempotencyKey,
                    OrganizationId = organizationId,
                    LocalEntityType = localEntityType,
                    LocalEntityId = localEntityId,
                    RefundKind = MarketplaceRefundKindConstants.Cancellation,
                    Status = ResolveInitialStatus(marketplaceBooking),
                    RequestedAt = preview.RequestedAt,
                    ReferenceTime = preview.ReferenceTime,
                    RefundPercentage = preview.RefundPercentage,
                    AppliedRuleMinutesBefore = preview.AppliedRuleMinutesBefore,
                    BaseAmount = preview.BaseAmount,
                    RefundAmount = preview.RefundAmount,
                    Currency = ResolveRefundCurrency(marketplaceBooking, preview),
                    PolicySnapshotJson = JsonSerializer.Serialize(policySnapshot),
                    CalculationResultJson = BuildCalculationResultJson(policySnapshot, preview, timezoneId),
                    TimezoneId = timezoneId,
                    RequestedByCustomerId = requestedByCustomer?.Id
                });
            await AddPaymentAllocationIfAvailableAsync(refund, marketplaceBooking, cancellationToken);
            marketplaceRefundEventService.Add(
                refund,
                MapInitialEventType(refund.Status),
                requestedByCustomer?.Id,
                refund.RequestedAt);

            logger.LogInformation(
                "Cancellation refund {RefundId} created for {LocalEntityType}/{LocalEntityId}; status={RefundStatus}; amount={RefundAmount}; paymentMethod={PaymentMethod}",
                refund.Id,
                refund.LocalEntityType,
                refund.LocalEntityId,
                refund.Status,
                refund.RefundAmount,
                marketplaceBooking.PaymentMethod);

            if (refund.Status == MarketplaceRefundStatusConstants.Requested)
            {
                logger.LogInformation(
                    "Queueing ProcessMarketplaceRefund for cancellation refund {RefundId}",
                    refund.Id);
                temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                    new ProcessMarketplaceRefundInput(refund.Id, requestedByCustomer?.Id),
                    repositoryFactory.UnitOfWork);
            }

            return refund;
        }

        logger.LogInformation(
            "Reusing existing cancellation refund {RefundId} for {LocalEntityType}/{LocalEntityId}; status={RefundStatus}",
            existingRefund.Id,
            existingRefund.LocalEntityType,
            existingRefund.LocalEntityId,
            existingRefund.Status);
        return existingRefund;
    }

    /// <summary>
    ///     Calculates a pro-rated subscription cancellation preview based on remaining days in the
    ///     current billing window. Unconsumed fraction = (windowEnd - now) / (windowEnd - windowStart).
    /// </summary>
    private MarketplaceRefundPreview GetSubscriptionProRatedPreview(
        string organizationId,
        MarketplaceBookingSubscriptionEntity subscription,
        RecurringBookingEntity? recurringBooking,
        DateTimeOffset requestedAt,
        bool hasConfirmedPayment)
    {
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking);
        var referenceTime = subscription.NextRenewalAt ?? subscription.StartedAt;

        if (!hasConfirmedPayment)
        {
            return new MarketplaceRefundPreview(organizationId,
                MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
                subscription.Id, requestedAt, referenceTime, false, 0, null, null, null,
                subscription.MarketplaceBooking.Currency);
        }

        // Determine the billing window for pro-rate calculation
        ArgumentNullException.ThrowIfNull(subscription.MarketplaceBooking.ProductVersion?.Product?.Organization);
        var (windowStart, windowEnd) = ResolveCurrentBillingWindow(
            subscription.StartedAt, requestedAt,
            subscription.MarketplaceBooking.ProductVersion.Product.Organization.BillingCycle);

        // Keep monetary and pro-ration arithmetic decimal. A TimeSpan's tick count is exact,
        // whereas TotalDays would introduce floating-point rounding at this boundary.
        var totalWindowTicks = (windowEnd - windowStart).Ticks;
        var remainingTicks = Math.Max(0L, (windowEnd - requestedAt).Ticks);
        var unconsumedFraction = totalWindowTicks > 0
            ? Math.Clamp((decimal)remainingTicks / totalWindowTicks, 0m, 1m)
            : 0m;

        // A materialized recurring series is more precise than elapsed time: occurrences
        // already delivered are not refundable, while future occurrences remain eligible.
        // Keep the time-based calculation as a compatibility fallback for projections that
        // have not loaded their generated booking instances.
        if (recurringBooking is { Bookings.Count: > 0 })
        {
            var occurrences = recurringBooking.Bookings
                .Where(item => !item.DeletedAt.HasValue && item.From >= windowStart && item.From < windowEnd)
                .ToList();
            if (occurrences.Count > 0)
            {
                var undeliveredOccurrences = occurrences.Count(item => item.From > requestedAt);
                unconsumedFraction = Math.Clamp((decimal)undeliveredOccurrences / occurrences.Count, 0m, 1m);
            }
        }

        var refundPercentage = Math.Clamp(
            (int)Math.Round(unconsumedFraction * 100m, MidpointRounding.AwayFromZero), 0, 100);

        var marketplaceBooking = recurringBooking?.MarketplaceBooking ?? subscription.MarketplaceBooking;
        var baseAmount = ResolveBaseAmount(marketplaceBooking);
        decimal? refundAmount = refundPercentage > 0 && baseAmount is not null
            ? Math.Round(baseAmount.Value * unconsumedFraction, 2, MidpointRounding.AwayFromZero)
            : null;

        logger.LogInformation(
            "Subscription pro-rate for {SubscriptionId}: window={WindowStart:O}..{WindowEnd:O} remaining={RemainingDays:F1}d/{TotalWindowDays:F1}d => {RefundPercentage}% refund={RefundAmount}",
            subscription.Id, windowStart, windowEnd, remainingTicks / (decimal)TimeSpan.TicksPerDay,
            totalWindowTicks / (decimal)TimeSpan.TicksPerDay, refundPercentage, refundAmount);

        return new MarketplaceRefundPreview(organizationId,
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            subscription.Id, requestedAt, referenceTime,
            refundPercentage > 0, refundPercentage, null, baseAmount, refundAmount,
            marketplaceBooking.Currency);
    }

    private static CancellationPolicySnapshot BuildPolicySnapshot(ProductPricing pricing, DateTimeOffset capturedAt) =>
        new(
            pricing.CancellationPolicyType.ToString(),
            pricing.CancellationRefundRules
                .Select(r => new CancellationRefundRuleSnapshot(r.MinutesBefore, r.RefundPercentage))
                .ToList(),
            capturedAt,
            pricing.Id);

    private static string BuildCalculationResultJson(
        CancellationPolicySnapshot policySnapshot,
        MarketplaceRefundPreview preview,
        string timezoneId)
    {
        var originalGrossAmount = preview.BaseAmount ?? 0m;
        var eligibleRefundAmount = preview.RefundAmount ?? 0m;
        var calculation = new MarketplaceRefundCalculationResult(
            originalGrossAmount,
            eligibleRefundAmount,
            originalGrossAmount - eligibleRefundAmount,
            0m,
            0m,
            eligibleRefundAmount,
            originalGrossAmount - eligibleRefundAmount,
            preview.IsRefundable ? "Cancellation policy calculation" : "Cancellation is not refundable under the policy",
            policySnapshot,
            preview.RequestedAt,
            preview.RequestedAt,
            preview.ReferenceTime,
            timezoneId);
        return JsonSerializer.Serialize(calculation);
    }

    private static string ResolveTimezoneId(IEnumerable<string?> timezoneIds) =>
        timezoneIds.FirstOrDefault(item => !string.IsNullOrWhiteSpace(item)) ?? TimeZoneInfo.Utc.Id;

    private static string ResolveInitialStatus(MarketplaceBookingEntity marketplaceBooking) =>
        string.Equals(marketplaceBooking.PaymentMethod, "BANK_TRANSFER", StringComparison.OrdinalIgnoreCase)
            ? MarketplaceRefundStatusConstants.UnderReview
            : MarketplaceRefundStatusConstants.Requested;

    private static string MapInitialEventType(string status) =>
        status == MarketplaceRefundStatusConstants.UnderReview
            ? MarketplaceRefundEventTypeConstants.UnderReview
            : MarketplaceRefundEventTypeConstants.Requested;

    private async Task AddPaymentAllocationIfAvailableAsync(
        MarketplaceRefund refund,
        MarketplaceBookingEntity marketplaceBooking,
        CancellationToken cancellationToken)
    {
        // Determine source payment reference — prefer Stripe session ID, fall back to booking ID
        var reference = marketplaceBooking.StripeCheckoutSession?.StripeCheckoutSessionId
                        ?? marketplaceBooking.Id;
        var provider = string.Equals(marketplaceBooking.PaymentMethod, "BANK_TRANSFER", StringComparison.OrdinalIgnoreCase)
            ? "BANK_TRANSFER"
            : "STRIPE";
        // Manual bank-transfer confirmation records the payment status, but generated
        // recurring marketplace bookings may not carry copied monetary totals. Use the same
        // canonical amount resolver as the refund preview so the allocation is not created
        // with a zero captured balance.
        var capturedAmount = ResolveBaseAmount(marketplaceBooking) ?? 0m;
        var reservedAmount = refund.RefundAmount ?? 0m;
        var source = await repositoryFactory.MarketplaceRefundRepository
            .GetSourceAllocationAsync(provider, reference, cancellationToken);
        if (source is null || string.IsNullOrWhiteSpace(source.Id))
        {
            source = repositoryFactory.MarketplaceRefundRepository.AddAllocation(new MarketplaceRefundPaymentAllocation
            {
                Id = randomHelper.Generate() + "-source",
                MarketplaceRefundId = refund.Id,
                SourcePaymentProvider = provider,
                SourcePaymentReference = reference,
                SourceCapturedAmount = capturedAmount,
                AllocatedRefundAmount = 0m,
                IsSourcePayment = true,
                Currency = refund.Currency ?? marketplaceBooking.Currency ?? "NZD"
            });
        }
        else if (provider != "BANK_TRANSFER" && source.SourceCapturedAmount != capturedAmount)
        {
            throw new InvalidOperationException("The captured source-payment amount does not match the existing allocation record.");
        }

        // Bank-transfer refunds are deliberately manual/under-review. There is no provider
        // capture balance that this service can safely reserve against, and cancellation must
        // not be blocked by a stale or incomplete local allocation row. The source row remains
        // available for the operator workflow; automatic balance enforcement stays enabled for
        // provider-backed refunds such as Stripe.
        if (provider == "BANK_TRANSFER")
        {
            return;
        }

        if (reservedAmount <= 0m)
        {
            return;
        }

        await repositoryFactory.MarketplaceRefundRepository.ReserveAllocationAsync(
            refund.Id,
            source.Id,
            reservedAmount,
            cancellationToken);
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
        var timezoneId = TimeZoneInfo.Utc.Id;
        var taxAmount = marketplaceBooking.TaxAmount ?? 0m;
        decimal? refundAmount = quote.IsRefundable && baseAmount is not null
            ? quote.CalculateRefundAmount(baseAmount.Value)
            : null;

        logger.LogInformation(
            "Refund calculation for {LocalEntityType}/{LocalEntityId}: refundable={IsRefundable} percentage={RefundPercentage} base={BaseAmount} tax={TaxAmount} eligible={EligibleRefundAmount} nonRefundable={NonRefundableAmount} currency={Currency} timezone={TimezoneId} requestedAt={RequestedAt} referenceTime={ReferenceTime} reason={CalculationReason}",
            localEntityType, localEntityId, quote.IsRefundable, quote.RefundPercentage, baseAmount, taxAmount, refundAmount,
            baseAmount - refundAmount, marketplaceBooking.Currency, timezoneId, requestedAt, referenceTime,
            quote.IsRefundable ? "Cancellation policy calculation" : "Cancellation is not refundable under the policy");

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
