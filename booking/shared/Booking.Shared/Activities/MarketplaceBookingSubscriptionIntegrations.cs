using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Temporalio.Activities;
using Constants = Booking.Shared.GraphQL.Constants;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.Activities;

public record AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput(string MarketplaceBookingSubscriptionId);

public record AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(bool Deleted, bool Ended);

public record ReleaseMarketplaceBookingSubscriptionResourcesInput(string MarketplaceBookingSubscriptionId);

public record ReleaseRecurringBookingResourcesInput(string RecurringBookingId);

public class MarketplaceBookingSubscriptionIntegrations(
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IRecurringBookingScheduleService recurringBookingScheduleService,
    IMarketplaceBookingService marketplaceBookingService,
    IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
    IProductVersionHelperService productVersionHelperService,
    ITemporalService temporalService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IMapper mapper,
    IRandomHelper randomHelper)
{
    [Activity]
    public async Task<AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse>
        AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(
            AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
            args.MarketplaceBookingSubscriptionId,
            cancellationToken);
        if (subscription is null || subscription.IsDeleted())
        {
            return new AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(true, true);
        }

        subscription = await UpdateMarketplaceBookingSubscriptionStateAsync(subscription, cancellationToken);

        if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() != MarketplaceBookingSubscriptionStatus.Active)
        {
            if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() == MarketplaceBookingSubscriptionStatus.Paused)
            {
                // Keep the workflow alive for paused and other non-deleted states so it can
                // re-check the subscription on the next daily cycle without needing a signal.
                return new AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(false, false);
            }

            return new AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(false, true);
        }

        var currentCycleRecurringBooking = await EnsureCurrentCycleRecurringBookingAsync(subscription, cancellationToken);
        if (subscription.RecurringBookings.All(item => item.Id != currentCycleRecurringBooking.Id))
        {
            subscription.RecurringBookings.Add(currentCycleRecurringBooking);
        }

        var now = timeProvider.GetUtcNow();
        var from = new DateTimeOffset(now.UtcDateTime.Date, TimeSpan.Zero);
        var hasAnyMoreRequiredBookingDays = false;

        foreach (var recurringBooking in subscription.RecurringBookings)
        {
            var recurringBookingEnded = await AdjustRecurringBookingAsync(recurringBooking, subscription, from, cancellationToken);
            hasAnyMoreRequiredBookingDays |= !recurringBookingEnded;
        }

        return new AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(
            false,
            ShouldEndCurrentCycleProcessing(subscription, from, hasAnyMoreRequiredBookingDays));
    }

    [Activity]
    public async Task ReleaseMarketplaceBookingSubscriptionResourcesAsync(ReleaseMarketplaceBookingSubscriptionResourcesInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
            args.MarketplaceBookingSubscriptionId,
            cancellationToken);
        if (subscription is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var from = new DateTimeOffset(now.UtcDateTime.Date, TimeSpan.Zero);

        foreach (var recurringBooking in subscription.RecurringBookings.Where(item => !item.IsDeleted()))
        {
            var existingBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
                recurringBooking.Id,
                from,
                null,
                cancellationToken);

            foreach (var existingBooking in existingBookings)
            {
                await marketplaceBookingService.DeleteAsync(existingBooking, subscription.DeletedByCustomer, false, cancellationToken);
            }
        }
    }

    [Activity]
    public async Task ReleaseRecurringBookingResourcesAsync(ReleaseRecurringBookingResourcesInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null)
        {
            return;
        }

        var from = recurringBooking.StartDate;
        var existingBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
            recurringBooking.Id,
            from,
            null,
            cancellationToken);

        foreach (var existingBooking in existingBookings)
        {
            await marketplaceBookingService.DeleteAsync(existingBooking, null, false, cancellationToken);
        }
    }

    private async Task<bool> AdjustRecurringBookingAsync(
        RecurringBooking recurringBooking,
        MarketplaceBookingSubscription subscription,
        DateTimeOffset from,
        CancellationToken cancellationToken)
    {
        recurringBooking = await EnsureRecurringBookingMarketplaceBookingLoadedAsync(recurringBooking, cancellationToken);

        var until = ResolvePlanningWindowEndExclusive(subscription);
        var existingBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
            recurringBooking.Id,
            from,
            null,
            cancellationToken);
        var reconciliationPlan = recurringBookingScheduleService.GetReconciliationPlan(
            recurringBooking,
            from,
            until,
            existingBookings);

        // Remove bookings that are no longer part of the recurring schedule before we
        // attempt any re-timing or resource repair.
        foreach (var existingBooking in reconciliationPlan.BookingsToRemove)
        {
            await marketplaceBookingService.DeleteAsync(existingBooking, null, false, cancellationToken);
        }

        var bookingsToRemoveIds = reconciliationPlan.BookingsToRemove.Select(item => item.Id).ToHashSet();
        var existingBookingsToRefresh = existingBookings
            .Where(item => !bookingsToRemoveIds.Contains(item.Id))
            .Where(item => item.HasRecurringInstanceOverrides != true)
            .ToList();

        var updatedByCustomer = recurringBooking.LastModifiedByCustomer ??
                                recurringBooking.CreatedByCustomer ??
                                subscription.LastModifiedByCustomer ??
                                subscription.CreatedByCustomer;
        ArgumentNullException.ThrowIfNull(updatedByCustomer);

        var customer = recurringBooking.InvolvedCustomers.Count == 1
            ? await repositoryFactory.CustomerRepository.GetByIdAsync(recurringBooking.InvolvedCustomers.First().Id, true, cancellationToken)
            : null;
        var preferredLocationId = existingBookingsToRefresh
            .Select(marketplaceBookingOpeningHoursService.ResolveLocation)
            .FirstOrDefault(item => item is not null)?.Id;
        var preferredResourceIds = await ResolvePreferredResourceIdsAsync(
            subscription,
            recurringBooking,
            existingBookingsToRefresh,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(recurringBooking.MarketplaceBooking);

        if (recurringBooking.MarketplaceBooking.ProductVersion.Type == ProductTypeConstants.Event)
        {
            throw new MarketplaceEventProductRecurringBookingNotSupported();
        }

        var requiredResourceCount =
            recurringBooking.MarketplaceBooking.Quantity * recurringBooking.MarketplaceBooking.ProductPricing.NumberOfResourcesToBook;
        var useOpeningHoursWindow = marketplaceBookingOpeningHoursService.ShouldUseLocationOpeningHoursWindow(
            recurringBooking.MarketplaceBooking.ProductPricing.PurchaseCadence);

        foreach (var existingBooking in existingBookingsToRefresh)
        {
            // Existing marketplace instances keep the original time window they were created with.
            // Reconciliation here only repairs resource assignment; it does not move the booking
            // to match later opening-hours changes.
            await marketplaceBookingService.AdjustRequiredResourcesAsync(existingBooking, cancellationToken);
        }

        foreach (var missingBookingDay in reconciliationPlan.MissingBookingDays)
        {
            if (ShouldSkipResourceMaterializationForTerminalPaymentStatus(recurringBooking.MarketplaceBooking))
            {
                return true;
            }

            var booking = mapper.MapTo(recurringBooking, missingBookingDay);
            booking.Id = randomHelper.Generate();
            if (useOpeningHoursWindow)
            {
                // Missing marketplace instances are created only for days where a location is open
                // and enough resources exist for the product tags.
                // The opening-hours service will prefer resource-level overridden availability
                // over the parent location opening hours when selecting the booking window.
                var dailyPlan = await marketplaceBookingOpeningHoursService.TryResolveDailyPlanAsync(
                    customer,
                    recurringBooking.MarketplaceBooking.ProductVersion,
                    recurringBooking.MarketplaceBooking.ProductPricing,
                    missingBookingDay,
                    requiredResourceCount,
                    ResolveRequiredResourceIds(subscription),
                    preferredResourceIds,
                    preferredLocationId,
                    cancellationToken);
                if (dailyPlan is null)
                {
                    continue;
                }

                booking.From = dailyPlan.From;
                booking.Until = dailyPlan.Until;
                booking.Schedules = [new BookingSchedule(booking.From, booking.Until)];
                booking.Resources = dailyPlan.Resources
                    .Select(item => new ResourceCustomersPair(
                        new Resource { Id = item.Id },
                        booking.InvolvedCustomers.ToList()))
                    .ToList();
            }

            var marketplaceBooking = mapper.MapTo(recurringBooking.MarketplaceBooking)!;
            marketplaceBooking.Id = randomHelper.Generate();
            marketplaceBooking.IsPaymentRequired = false;
            marketplaceBooking.PaymentStatus = PaymentStatus.NotSet;
            marketplaceBooking.ProductPricing = marketplaceBooking.ProductPricing with
            {
                // Generated recurring marketplace instances always use the recurring-compatible
                // daily booking cadence. The purchase cadence remains on the parent subscription/template.
                BookingCadence = ResolveInstanceBookingCadence(marketplaceBooking.ProductPricing.PurchaseCadence)
            };

            booking.MarketplaceBooking = marketplaceBooking;

            await marketplaceBookingService.AddAsync(
                booking,
                recurringBooking.InvolvedCustomers.First(),
                recurringBooking.InvolvedOrganizations,
                recurringBooking.InvolvedTeams,
                recurringBooking,
                cancellationToken);
        }

        return !reconciliationPlan.HasMoreRequiredBookingDays;
    }

    private async Task<ICollection<string>> ResolvePreferredResourceIdsAsync(
        MarketplaceBookingSubscription subscription,
        RecurringBooking recurringBooking,
        ICollection<Database.Entities.Booking> existingBookingsToRefresh,
        CancellationToken cancellationToken)
    {
        // Prefer the latest resources already assigned in the current cycle first.
        var currentCyclePreferredResourceIds = existingBookingsToRefresh
            .OrderByDescending(item => item.From)
            .SelectMany(item => item.InvolvedResources.Select(resource => resource.Id))
            .Distinct()
            .ToList();
        if (currentCyclePreferredResourceIds.Count > 0)
        {
            return currentCyclePreferredResourceIds;
        }

        // Auto-renew creates a new recurring booking cycle with no instances yet. In that case
        // carry forward the latest assigned resources from the most recent previous cycle so the
        // opening-hours service can keep the cadence sticky when those resources are still valid.
        var previousRecurringBookings = subscription.RecurringBookings
            .Where(item => !item.IsDeleted())
            .Where(item => item.Id != recurringBooking.Id)
            .OrderByDescending(item => item.StartDate)
            .ToList();

        foreach (var previousRecurringBooking in previousRecurringBookings)
        {
            var previousBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
                previousRecurringBooking.Id,
                previousRecurringBooking.StartDate,
                null,
                cancellationToken);
            var previousPreferredResourceIds = previousBookings
                .OrderByDescending(item => item.From)
                .SelectMany(item => item.InvolvedResources.Select(resource => resource.Id))
                .Distinct()
                .ToList();
            if (previousPreferredResourceIds.Count > 0)
            {
                return previousPreferredResourceIds;
            }
        }

        return [];
    }

    private static ICollection<string> ResolveRequiredResourceIds(MarketplaceBookingSubscription subscription) =>
        subscription.RequestedResources
            .Select(item => item.Id)
            .Distinct()
            .ToList();

    private async Task<RecurringBooking> EnsureCurrentCycleRecurringBookingAsync(
        MarketplaceBookingSubscription subscription,
        CancellationToken cancellationToken)
    {
        var purchaseCadence = subscription.MarketplaceBooking.ProductPricing.PurchaseCadence;
        var cycleEndExclusive = subscription.NextRenewalAt ?? ResolveNextRenewalAt(subscription.StartedAt, purchaseCadence);
        var cycleStart = ResolveCycleStart(cycleEndExclusive, purchaseCadence);
        var cycleEnd = cycleEndExclusive.AddDays(-1);

        var existingRecurringBooking = subscription.RecurringBookings
            .Where(item => !item.IsDeleted())
            .FirstOrDefault(item =>
                item.StartDate.UtcDateTime.Date == cycleStart.UtcDateTime.Date &&
                item.EndDate.HasValue &&
                item.EndDate.Value.UtcDateTime.Date == cycleEnd.UtcDateTime.Date);
        if (existingRecurringBooking is not null)
        {
            await EnsureCurrentCyclePaymentWorkflowStartedAsync(existingRecurringBooking, cancellationToken);

            return existingRecurringBooking;
        }

        var recurringMarketplaceBooking = CreateRecurringMarketplaceBookingTemplate(subscription);

        var recurringBooking = repositoryFactory.RecurringBookingRepository.Add(
            new RecurringBooking
            {
                Id = randomHelper.Generate(),
                From = ResolveRecurringInstanceFrom(subscription),
                Until = ResolveRecurringInstanceUntil(subscription),
                Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
                Channel = BookingChannel.Marketplace.ToBookingChannel(),
                Frequency = BookingFrequencyConstants.Daily,
                Interval = 1,
                ByMonthDay = null,
                BySetPosition = null,
                ByWeekDays = [],
                EndType = RecurringBookingEndTypeConstants.UntilDate,
                StartDate = cycleStart,
                EndDate = cycleEnd,
                OccurrenceCount = null,
                SkippedDates = [],
                InvolvedCustomers = subscription.InvolvedCustomers,
                InvolvedOrganizations = subscription.InvolvedOrganizations,
                InvolvedTeams = subscription.InvolvedTeams,
                CreatedByCustomer = subscription.CreatedByCustomer ?? subscription.InvolvedCustomers.FirstOrDefault(),
                LastModifiedByCustomer = null,
                DeletedByCustomer = null,
                MarketplaceBookingSubscription = subscription,
                MarketplaceBooking = recurringMarketplaceBooking
            });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (ShouldStartRecurringBookingCardPaymentWorkflow(recurringMarketplaceBooking))
        {
            await temporalService.StartWorkflowPayRecurringBookingViaCardAsync(
                new PayRecurringBookingViaCardInput(
                    recurringBooking.Id,
                    recurringMarketplaceBooking.PaymentExpiry,
                    recurringMarketplaceBooking.InvoiceEmailList.ToList()),
                cancellationToken);
        }
        else if (ShouldStartRecurringBookingBankTransferPaymentWorkflow(recurringMarketplaceBooking))
        {
            await temporalService.StartWorkflowPayRecurringBookingViaBankTransferAsync(
                new PayRecurringBookingViaBankTransferInput(
                    recurringBooking.Id,
                    recurringMarketplaceBooking.PaymentExpiry,
                    recurringMarketplaceBooking.InvoiceEmailList.ToList()),
                cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
            Constants.MarketplaceBookingSubscriptionTopicName,
            subscription.Id,
            cancellationToken);

        return recurringBooking;
    }

    private async Task EnsureCurrentCyclePaymentWorkflowStartedAsync(
        RecurringBooking recurringBooking,
        CancellationToken cancellationToken)
    {
        recurringBooking = await EnsureRecurringBookingMarketplaceBookingLoadedAsync(recurringBooking, cancellationToken);
        var marketplaceBooking = recurringBooking.MarketplaceBooking;

        if (marketplaceBooking is null ||
            (!ShouldStartRecurringBookingCardPaymentWorkflow(marketplaceBooking) &&
             !ShouldStartRecurringBookingBankTransferPaymentWorkflow(marketplaceBooking)))
        {
            return;
        }

        if (ShouldStartRecurringBookingCardPaymentWorkflow(marketplaceBooking))
        {
            if (marketplaceBooking.StripeCheckoutSession is not null)
            {
                return;
            }

            await temporalService.StartWorkflowPayRecurringBookingViaCardAsync(
                new PayRecurringBookingViaCardInput(
                    recurringBooking.Id,
                    marketplaceBooking.PaymentExpiry,
                    marketplaceBooking.InvoiceEmailList.ToList()),
                cancellationToken);
        }
        else if (ShouldStartRecurringBookingBankTransferPaymentWorkflow(marketplaceBooking))
        {
            if (!string.IsNullOrWhiteSpace(marketplaceBooking.InvoiceUrl))
            {
                return;
            }

            await temporalService.StartWorkflowPayRecurringBookingViaBankTransferAsync(
                new PayRecurringBookingViaBankTransferInput(
                    recurringBooking.Id,
                    marketplaceBooking.PaymentExpiry,
                    marketplaceBooking.InvoiceEmailList.ToList()),
                cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<RecurringBooking> EnsureRecurringBookingMarketplaceBookingLoadedAsync(
        RecurringBooking recurringBooking,
        CancellationToken cancellationToken)
    {
        if (recurringBooking.MarketplaceBooking is not null)
        {
            return recurringBooking;
        }

        // Some subscription aggregate query shapes include the recurring booking identity
        // but not its cycle marketplace booking. Reload the recurring booking directly so
        // all downstream payment and reconciliation logic runs against the persisted template.
        return await repositoryFactory.RecurringBookingRepository.GetByIdAsync(recurringBooking.Id, cancellationToken) ??
               recurringBooking;
    }

    private async Task<MarketplaceBookingSubscription> UpdateMarketplaceBookingSubscriptionStateAsync(
        MarketplaceBookingSubscription subscription,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var subscriptionStatus = subscription.Status.ToMarketplaceBookingSubscriptionStatus();
        var nextRenewalAt = subscription.NextRenewalAt ??
                            ResolveNextRenewalAt(subscription.StartedAt, subscription.MarketplaceBooking.ProductPricing.PurchaseCadence);
        var hasChanges = subscription.NextRenewalAt != nextRenewalAt;

        subscription.NextRenewalAt = nextRenewalAt;

        if (subscriptionStatus == MarketplaceBookingSubscriptionStatus.Paused)
        {
            if (!hasChanges)
            {
                return subscription;
            }

            subscription = repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

            return subscription;
        }

        while (subscription.NextRenewalAt.HasValue && subscription.NextRenewalAt.Value <= now)
        {
            if (!subscription.AutoRenew || !subscription.MarketplaceBooking.ProductPricing.SupportsSubscriptionAutoRenewal)
            {
                if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() != MarketplaceBookingSubscriptionStatus.Expired)
                {
                    subscription.Status = MarketplaceBookingSubscriptionStatus.Expired.ToMarketplaceBookingSubscriptionStatus();
                    hasChanges = true;
                }

                break;
            }

            var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(subscription.ProductVersion.Id, cancellationToken);
            if (productVersion?.PricingOptions is null)
            {
                if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() != MarketplaceBookingSubscriptionStatus.RenewalFailed)
                {
                    subscription.Status = MarketplaceBookingSubscriptionStatus.RenewalFailed.ToMarketplaceBookingSubscriptionStatus();
                    hasChanges = true;
                }

                break;
            }

            var renewedProductPricing =
                productVersionHelperService.FindMatchingPricing(productVersion.PricingOptions, subscription.MarketplaceBooking.ProductPricing);
            if (renewedProductPricing is null || !renewedProductPricing.SupportsSubscriptionAutoRenewal)
            {
                if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() != MarketplaceBookingSubscriptionStatus.RenewalFailed)
                {
                    subscription.Status = MarketplaceBookingSubscriptionStatus.RenewalFailed.ToMarketplaceBookingSubscriptionStatus();
                    hasChanges = true;
                }

                break;
            }

            subscription.MarketplaceBooking.ProductPricing = renewedProductPricing;
            subscription.ProductVersion = productVersion;
            subscription.Status = MarketplaceBookingSubscriptionStatus.Active.ToMarketplaceBookingSubscriptionStatus();
            subscription.NextRenewalAt = ResolveNextRenewalAt(subscription.NextRenewalAt.Value, renewedProductPricing.PurchaseCadence);
            hasChanges = true;
        }

        if (!hasChanges)
        {
            return subscription;
        }

        subscription = repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return subscription;
    }

    private MarketplaceBooking CreateRecurringMarketplaceBookingTemplate(MarketplaceBookingSubscription subscription)
    {
        var marketplaceBooking = subscription.MarketplaceBooking;
        var bookingCadence = ResolveInstanceBookingCadence(marketplaceBooking.ProductPricing.PurchaseCadence);
        var isUpfront = marketplaceBooking.ProductPricing.BillingMode == ProductPricingBillingMode.Upfront;
        var paymentExpiry = isUpfront
            ? timeProvider.GetUtcNow()
                .TrimAllAfterSeconds()
                .AddMinutes(GetBookingPaymentExpiryInMinutes(marketplaceBooking.ProductPricing, marketplaceBooking.PaymentMethod.ToPaymentMethod()))
            : default;

        return repositoryFactory.MarketplaceBookingRepository.Add(
            new MarketplaceBooking
            {
                Id = randomHelper.Generate(),
                // Both upfront and in-arrears subscription cycles begin unpaid. Upfront
                // cycles later create hosted checkout immediately, while in-arrears cycles
                // stay pending until the organization bills and confirms payment.
                PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
                IsPaymentRequired = isUpfront,
                Quantity = marketplaceBooking.Quantity,
                ProductPricing = marketplaceBooking.ProductPricing with { BookingCadence = bookingCadence },
                PaymentMethod = marketplaceBooking.PaymentMethod,
                PaymentExpiry = paymentExpiry,
                TotalAmountExcludeTax = marketplaceBooking.TotalAmountExcludeTax,
                TaxAmount = marketplaceBooking.TaxAmount,
                TaxRatePercentage = marketplaceBooking.TaxRatePercentage,
                TotalAmount = marketplaceBooking.TotalAmount,
                Currency = marketplaceBooking.Currency,
                InvoiceUrl = null,
                InvoiceNumber = null,
                CheckoutReturnUrl = marketplaceBooking.CheckoutReturnUrl,
                InvoiceEmailList = marketplaceBooking.InvoiceEmailList,
                BillingMode = marketplaceBooking.BillingMode,
                ProductVersion = subscription.MarketplaceBooking.ProductVersion,
                PaidByCustomer = subscription.MarketplaceBooking.PaidByCustomer,
                PaidByOrganization = subscription.MarketplaceBooking.PaidByOrganization,
                StripeCheckoutSession = null
            });
    }

    private static bool ShouldStartRecurringBookingCardPaymentWorkflow(MarketplaceBooking marketplaceBooking) =>
        marketplaceBooking.IsPaymentRequired &&
        marketplaceBooking.ProductPricing.BillingMode == ProductPricingBillingMode.Upfront &&
        marketplaceBooking.PaymentMethod.ToPaymentMethod() == PaymentMethod.Card;

    private static bool ShouldStartRecurringBookingBankTransferPaymentWorkflow(MarketplaceBooking marketplaceBooking) =>
        marketplaceBooking.IsPaymentRequired &&
        marketplaceBooking.ProductPricing.BillingMode == ProductPricingBillingMode.Upfront &&
        marketplaceBooking.PaymentMethod.ToPaymentMethod() == PaymentMethod.BankTransfer;

    private static bool ShouldSkipResourceMaterializationForTerminalPaymentStatus(MarketplaceBooking? marketplaceBooking) =>
        marketplaceBooking is not null &&
        marketplaceBooking.ProductPricing.BillingMode == ProductPricingBillingMode.Upfront &&
        marketplaceBooking.PaymentStatus.ToPaymentStatus() is PaymentStatus.Expired or PaymentStatus.Rejected or PaymentStatus.RecordNeverCreated;

    private static int GetBookingPaymentExpiryInMinutes(ProductPricing pricing, PaymentMethod paymentMethod) =>
        paymentMethod switch
        {
            PaymentMethod.Card => pricing.MaxAllowedResourcesLockTimePaidViaCard,
            PaymentMethod.BankTransfer => pricing.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentMethod), paymentMethod, null)
        };

    private static DateTimeOffset ResolvePlanningWindowEndExclusive(MarketplaceBookingSubscription subscription) =>
        // The subscription owns the billing period horizon. Reconciliation should therefore
        // always plan against the current subscription cycle, even if an existing recurring
        // booking record was created earlier with stale end-date data.
        subscription.NextRenewalAt ?? ResolveNextRenewalAt(subscription.StartedAt, subscription.MarketplaceBooking.ProductPricing.PurchaseCadence);

    private static bool ShouldEndCurrentCycleProcessing(
        MarketplaceBookingSubscription subscription,
        DateTimeOffset from,
        bool hasAnyMoreRequiredBookingDays)
    {
        // Even when the full cycle has already been materialized, the workflow still needs
        // to wake up daily to repair resources or react to opening-hours changes. The only
        // time this cycle can stop naturally is the final day of a non-renewing subscription
        // once there are no further booking days left to create in the current cycle.
        if (hasAnyMoreRequiredBookingDays)
        {
            return false;
        }

        if (subscription.AutoRenew && subscription.MarketplaceBooking.ProductPricing.SupportsSubscriptionAutoRenewal)
        {
            return false;
        }

        var cycleEndExclusive = subscription.NextRenewalAt ??
                                ResolveNextRenewalAt(subscription.StartedAt, subscription.MarketplaceBooking.ProductPricing.PurchaseCadence);
        var lastCycleDay = cycleEndExclusive.AddDays(-1).UtcDateTime.Date;

        return from.UtcDateTime.Date >= lastCycleDay;
    }

    private static DateTimeOffset ResolveCycleStart(DateTimeOffset cycleEndExclusive, ProductPricingCadence cadence) =>
        cadence switch
        {
            ProductPricingCadence.Weekly => cycleEndExclusive.AddDays(-7),
            ProductPricingCadence.Fortnightly => cycleEndExclusive.AddDays(-14),
            ProductPricingCadence.Monthly => cycleEndExclusive.AddMonths(-1),
            ProductPricingCadence.TwoMonths => cycleEndExclusive.AddMonths(-2),
            ProductPricingCadence.Quarterly => cycleEndExclusive.AddMonths(-3),
            ProductPricingCadence.FourMonths => cycleEndExclusive.AddMonths(-4),
            ProductPricingCadence.FiveMonths => cycleEndExclusive.AddMonths(-5),
            ProductPricingCadence.SixMonths => cycleEndExclusive.AddMonths(-6),
            ProductPricingCadence.Yearly => cycleEndExclusive.AddYears(-1),
            _ => cycleEndExclusive.AddDays(-1)
        };

    private static DateTimeOffset ResolveNextRenewalAt(DateTimeOffset start, ProductPricingCadence cadence) =>
        cadence switch
        {
            ProductPricingCadence.Weekly => start.AddDays(7),
            ProductPricingCadence.Fortnightly => start.AddDays(14),
            ProductPricingCadence.Monthly => start.AddMonths(1),
            ProductPricingCadence.TwoMonths => start.AddMonths(2),
            ProductPricingCadence.Quarterly => start.AddMonths(3),
            ProductPricingCadence.FourMonths => start.AddMonths(4),
            ProductPricingCadence.FiveMonths => start.AddMonths(5),
            ProductPricingCadence.SixMonths => start.AddMonths(6),
            ProductPricingCadence.Yearly => start.AddYears(1),
            _ => start.AddDays(1)
        };

    private static DateTimeOffset ResolveRecurringInstanceFrom(MarketplaceBookingSubscription subscription) =>
        subscription.StartedAt;

    private static DateTimeOffset ResolveRecurringInstanceUntil(MarketplaceBookingSubscription subscription)
    {
        var from = ResolveRecurringInstanceFrom(subscription);

        // For day-based subscriptions the workflow later replaces the concrete booking window
        // with the location's actual opening hours. Shorter cadences keep their explicit time.
        return ResolveInstanceBookingCadence(subscription.MarketplaceBooking.ProductPricing.PurchaseCadence) switch
        {
            ProductPricingCadence.PerMinute => from.AddMinutes(1),
            ProductPricingCadence.Per15Minutes => from.AddMinutes(15),
            ProductPricingCadence.Per30Minutes => from.AddMinutes(30),
            ProductPricingCadence.PerHour => from.AddHours(1),
            ProductPricingCadence.HalfDay => from.AddHours(4),
            _ => new DateTimeOffset(from.UtcDateTime.Date.AddDays(1).AddTicks(-1), TimeSpan.Zero)
        };
    }

    private static ProductPricingCadence ResolveInstanceBookingCadence(ProductPricingCadence purchaseCadence) =>
        purchaseCadence switch
        {
            ProductPricingCadence.PerMinute => ProductPricingCadence.PerMinute,
            ProductPricingCadence.Per15Minutes => ProductPricingCadence.Per15Minutes,
            ProductPricingCadence.Per30Minutes => ProductPricingCadence.Per30Minutes,
            ProductPricingCadence.PerHour => ProductPricingCadence.PerHour,
            ProductPricingCadence.HalfDay => ProductPricingCadence.HalfDay,
            // Day-or-longer subscriptions are materialized one day at a time under the
            // recurring marketplace flow while the purchase cadence stays on the parent object.
            _ => ProductPricingCadence.Daily
        };
}
