using System.Data;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;
using Constants = Booking.Shared.GraphQL.Constants;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using BookingModel = Booking.Shared.Models.Booking;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.Activities;

public record AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput(string MarketplaceBookingSubscriptionId);

public record AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(bool Deleted, bool Ended);

public record ReleaseMarketplaceBookingSubscriptionResourcesInput(string MarketplaceBookingSubscriptionId);

public record ReleaseRecurringBookingResourcesInput(
    string RecurringBookingId,
    string FailureCategory = MarketplaceBookingFailureCategoryConstants.PaymentExpired);

public class MarketplaceBookingSubscriptionIntegrations(
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider,
    IRecurringBookingScheduleService recurringBookingScheduleService,
    IMarketplaceBookingService marketplaceBookingService,
    IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
    IProductVersionHelperService productVersionHelperService,
    ITemporalService temporalService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    ISpacesBookingQuotaService spacesBookingQuotaService,
    IMarketplaceBookingAvailableDaysService marketplaceBookingAvailableDaysService,
    IMarketplaceBookingWeeklyDaySelectionService marketplaceBookingWeeklyDaySelectionService,
    IMarketplaceBookingFailureService marketplaceBookingFailureService,
    IDbTransactionBuilder transactionBuilder,
    ICachedBookingService cachedBookingService,
    ILogger<MarketplaceBookingSubscriptionIntegrations> logger)
{
    [Activity]
    public async Task<AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse>
        AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsync(AdjustRequiredResourcesForMarketplaceBookingSubscriptionInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        logger.LogInformation(
            "Reconciling marketplace booking subscription {MarketplaceBookingSubscriptionId}",
            args.MarketplaceBookingSubscriptionId);
        var subscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdAsync(
            args.MarketplaceBookingSubscriptionId,
            cancellationToken);
        if (subscription is null || subscription.IsDeleted())
        {
            logger.LogInformation(
                "Marketplace booking subscription {MarketplaceBookingSubscriptionId} no longer exists or was deleted",
                args.MarketplaceBookingSubscriptionId);
            return new AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(true, true);
        }

        foreach (var organization in subscription.InvolvedOrganizations
                     .Where(item => item.Type == OrganizationTypeConstants.Marketplace)
                     .DistinctBy(item => item.Id))
        {
            var accessDecision = await spacesBookingQuotaService.EvaluateAccessAsync(
                organization.Id,
                SpacesAccessAction.CreateOrModify,
                cancellationToken);
            if (!accessDecision.Allowed)
            {
                logger.LogInformation(
                    "Marketplace booking subscription {MarketplaceBookingSubscriptionId} is waiting for spaces access for organization {OrganizationId}",
                    subscription.Id,
                    organization.Id);
                // Keep the workflow alive so a paid upgrade resumes reconciliation without
                // recreating the subscription or its existing configuration.
                return new AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(false, false);
            }
        }

        subscription = await UpdateMarketplaceBookingSubscriptionStateAsync(subscription, cancellationToken);

        if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() != MarketplaceBookingSubscriptionStatus.Active)
        {
            logger.LogInformation(
                "Marketplace booking subscription {MarketplaceBookingSubscriptionId} is not active; status is {Status}",
                subscription.Id,
                subscription.Status);
            if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() == MarketplaceBookingSubscriptionStatus.Paused)
            {
                // Keep the workflow alive for paused and other non-deleted states so it can
                // re-check the subscription on the next daily cycle without needing a signal.
                return new AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(false, false);
            }

            return new AdjustRequiredResourcesForMarketplaceBookingSubscriptionAsyncResponse(false, true);
        }

        var currentCycleRecurringBooking = await EnsureCurrentCycleRecurringBookingAsync(subscription, cancellationToken);
        logger.LogInformation(
            "Marketplace booking subscription {MarketplaceBookingSubscriptionId} is reconciling recurring booking {RecurringBookingId}",
            subscription.Id,
            currentCycleRecurringBooking.Id);
        if (subscription.RecurringBookings.All(item => item.Id != currentCycleRecurringBooking.Id))
        {
            subscription.RecurringBookings.Add(currentCycleRecurringBooking);
        }

        // Creating or selecting the current cycle changes the authoritative payment source
        // for the subscription purchase. Refresh the durable history row before further
        // reconciliation so payment, amount, currency, and activity stay in sync.
        await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingSubscriptionAsync(
            subscription, null, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var from = timeProvider.GetUtcNow();
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
        var from = now.StartOfDay();
        // Immediate cancellation already marks the subscription as cancelled in the API service,
        // but the daily adjustment loop may still be finishing work from a previously loaded
        // active snapshot. Re-stamp the terminal state here, in the same path that releases
        // resources and cancels Xero invoices, so the final persisted subscription state matches
        // the cleanup that just happened.
        subscription.CancelledAt ??= now;
        subscription.Status = MarketplaceBookingSubscriptionStatus.Cancelled.ToMarketplaceBookingSubscriptionStatus();
        subscription.AutoRenew = false;
        subscription.CancelAtPeriodEnd = false;
        repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);

        foreach (var recurringBooking in subscription.RecurringBookings.Where(item => !item.IsDeleted()))
        {
            await CancelRecurringBookingBillingAsync(recurringBooking, cancellationToken);

            var existingBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
                recurringBooking.Id,
                from,
                null,
                cancellationToken);

            foreach (var existingBooking in existingBookings)
            {
                await marketplaceBookingService.DeleteAsync(existingBooking, subscription.DeletedByCustomer, false, null, false, cancellationToken);
            }

            recurringBooking.DeletedByCustomer = subscription.DeletedByCustomer;
            repositoryFactory.RecurringBookingRepository.Update(recurringBooking);
            repositoryFactory.RecurringBookingRepository.Remove(recurringBooking);
        }

        await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingSubscriptionAsync(
            subscription, null, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        // Cleanup happens asynchronously after the original delete mutation returns, so publish the
        // subscription topic again once release is complete. This lets the UI move from the
        // optimistic/local cancelled state to the fully persisted post-cleanup subscription state.
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
            Constants.MarketplaceBookingSubscriptionTopicName,
            subscription.Id,
            cancellationToken);
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

        await accountingInvoiceCancellationService.CancelRecurringBookingAsync(recurringBooking, cancellationToken);
        await MarkRecurringBookingPaymentAsTerminalAsync(recurringBooking, cancellationToken);
        await MarkSubscriptionPaymentAsTerminalAsync(recurringBooking, args.FailureCategory, cancellationToken);

        await marketplaceBookingFailureService.FinalizeAsync(
            new MarketplaceBookingFailureFinalization(
                null,
                args.FailureCategory,
                MarketplaceBookingFailureScopeConstants.RecurringCycle,
                timeProvider.GetUtcNow(),
                null,
                recurringBooking.Id,
                recurringBooking.MarketplaceBookingSubscription?.Id,
                recurringBooking.StartDate,
                recurringBooking.EndDate,
                recurringBooking.RequestedResources.Select(item => item.Id).ToList(),
                MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription,
                null,
                "Recurring booking payment was not completed before resources were released.",
                recurringBooking.CreatedByCustomer?.Id,
                []),
            cancellationToken);

        var from = timeProvider.GetUtcNow().StartOfDay();
        var existingBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdAsync(
            recurringBooking.Id,
            from,
            null,
            cancellationToken);

        foreach (var existingBooking in existingBookings)
        {
            await marketplaceBookingService.DeleteAsync(existingBooking, null, false, null, false, cancellationToken);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        if (recurringBooking.MarketplaceBookingSubscription is not null)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                recurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }

        logger.LogInformation(
            "Released recurring marketplace booking resources after terminal payment outcome. RecurringBookingId={RecurringBookingId}, SubscriptionId={SubscriptionId}",
            recurringBooking.Id,
            recurringBooking.MarketplaceBookingSubscription?.Id);
    }

    private async Task CancelRecurringBookingBillingAsync(RecurringBooking recurringBooking, CancellationToken cancellationToken)
    {
        if (recurringBooking.MarketplaceBooking is null)
        {
            return;
        }

        if (recurringBooking.MarketplaceBooking.IsPaymentRequired)
        {
            switch (recurringBooking.MarketplaceBooking.PaymentMethod.ToPaymentMethod())
            {
                case PaymentMethod.Card:
                    await temporalService.SignalPayRecurringBookingViaCardWorkflowDeleteRecurringBookingAsync(recurringBooking.Id, cancellationToken);
                    break;

                case PaymentMethod.BankTransfer:
                    await temporalService.SignalPayRecurringBookingViaBankTransferWorkflowDeleteRecurringBookingAsync(
                        recurringBooking.Id,
                        cancellationToken);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case.");
            }
        }

        await accountingInvoiceCancellationService.CancelRecurringBookingAsync(recurringBooking, cancellationToken);
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
            await marketplaceBookingService.DeleteAsync(existingBooking, null, false, null, false, cancellationToken);
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

        var isInitialSeriesMaterialization = existingBookings.Count == 0 &&
                                             recurringBooking.StartDate.UtcDateTime.Date == subscription.StartedAt.UtcDateTime.Date;
        var initialDailyPlans = new Dictionary<DateOnly, MarketplaceBookingDailyPlan>();
        if (isInitialSeriesMaterialization && useOpeningHoursWindow)
        {
            foreach (var missingBookingDay in reconciliationPlan.MissingBookingDays)
            {
                if (!marketplaceBookingAvailableDaysService.IsAvailableOnBookingDate(
                        recurringBooking.MarketplaceBooking.ProductPricing,
                        missingBookingDay))
                {
                    continue;
                }

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
                if (dailyPlan is not null)
                {
                    initialDailyPlans.Add(missingBookingDay, dailyPlan);
                    continue;
                }

                await FinalizeInitialSeriesAvailabilityFailureAsync(
                    subscription,
                    recurringBooking,
                    cancellationToken);
                return true;
            }
        }

        await using var initialSeriesTransaction = isInitialSeriesMaterialization
            ? await transactionBuilder.BeginTransactionAsync(
                repositoryFactory.UnitOfWork,
                IsolationLevel.Serializable,
                cancellationToken)
            : null;
        var createdInitialBookings = new List<BookingModel>();
        foreach (var missingBookingDay in reconciliationPlan.MissingBookingDays)
        {
            if (ShouldSkipResourceMaterializationForTerminalPaymentStatus(recurringBooking.MarketplaceBooking))
            {
                return true;
            }

            if (!marketplaceBookingAvailableDaysService.IsAvailableOnBookingDate(
                    recurringBooking.MarketplaceBooking.ProductPricing,
                    missingBookingDay))
            {
                logger.LogInformation(
                    "Skipped recurring marketplace booking candidate for unavailable price day. SubscriptionId: {SubscriptionId}, RecurringBookingId: {RecurringBookingId}, PricingId: {PricingId}, LocalDate: {LocalDate}",
                    subscription.Id,
                    recurringBooking.Id,
                    recurringBooking.MarketplaceBooking.ProductPricing.Id,
                    missingBookingDay);
                continue;
            }

            var booking = entityMapper.MapTo(recurringBooking, missingBookingDay);
            booking.Id = randomHelper.Generate();
            var allowAutomaticResourceAssignment = true;
            if (useOpeningHoursWindow)
            {
                // Missing marketplace instances are created only for days when a location is open
                // and enough resources exist for the product tags.
                // The opening-hours service will prefer resource-level overridden availability
                // over the parent location opening hours when selecting the booking window.
                var dailyPlan = initialDailyPlans.TryGetValue(missingBookingDay, out var initialDailyPlan)
                    ? initialDailyPlan
                    : await marketplaceBookingOpeningHoursService.TryResolveDailyPlanAsync(
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
                    logger.LogWarning(
                        "Finalizing recurring marketplace booking availability failure because no complete daily allocation is available. SubscriptionId={SubscriptionId}, RecurringBookingId={RecurringBookingId}, BookingDate={BookingDate}",
                        subscription.Id,
                        recurringBooking.Id,
                        missingBookingDay);

                    var failure = await marketplaceBookingFailureService.FinalizeAsync(
                        new MarketplaceBookingFailureFinalization(
                            null,
                            MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
                            MarketplaceBookingFailureScopeConstants.RecurringOccurrence,
                            timeProvider.GetUtcNow(),
                            null,
                            recurringBooking.Id,
                            subscription.Id,
                            booking.From,
                            booking.Until,
                            ResolveRequiredResourceIds(subscription),
                            MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription,
                            null,
                            "The recurring booking occurrence could not be allocated because the requested capacity is no longer available.",
                            recurringBooking.CreatedByCustomer?.Id,
                            []),
                        cancellationToken);
                    await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                    await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                        Constants.MarketplaceBookingSubscriptionTopicName,
                        subscription.Id,
                        cancellationToken);
                    logger.LogInformation(
                        "Finalized recurring marketplace booking availability failure. FailureId={FailureId}, SubscriptionId={SubscriptionId}, RecurringBookingId={RecurringBookingId}, BookingDate={BookingDate}",
                        failure.Id,
                        subscription.Id,
                        recurringBooking.Id,
                        missingBookingDay);
                    continue;
                }

                booking.From = dailyPlan.From;
                booking.Until = dailyPlan.Until;
                booking.Schedules = [new BookingSchedule(booking.From, booking.Until)];
                booking.Resources = dailyPlan.Resources
                    .Select(item => new ResourceCustomersPair(
                        new Resource
                        {
                            Id = item.Id,
                        },
                        booking.InvolvedCustomers.ToList()))
                    .ToList();
            }

            var marketplaceBooking = entityMapper.MapTo(recurringBooking.MarketplaceBooking)!;
            marketplaceBooking.Id = randomHelper.Generate();
            marketplaceBooking.IsPaymentRequired = false;
            marketplaceBooking.PaymentStatus = PaymentStatus.NotSet;
            marketplaceBooking.ProductPricing = marketplaceBooking.ProductPricing with
            {
                // Generated recurring marketplace instances always use the recurring-compatible
                // daily booking cadence. The purchase cadence remains on the parent subscription/template.
                BookingCadence = ResolveInstanceBookingCadence(marketplaceBooking.ProductPricing.PurchaseCadence),
            };

            booking.MarketplaceBooking = marketplaceBooking;

            try
            {
                var createdBooking = await marketplaceBookingService.AddAsync(
                    booking,
                    recurringBooking.InvolvedCustomers.First(),
                    recurringBooking.InvolvedOrganizations.ToList(),
                    recurringBooking.InvolvedTeams.ToList(),
                    recurringBooking,
                    allowAutomaticResourceAssignment,
                    !isInitialSeriesMaterialization,
                    isInitialSeriesMaterialization,
                    cancellationToken);
                if (isInitialSeriesMaterialization)
                {
                    createdInitialBookings.Add(createdBooking);
                }
            }
            catch (MarketplaceBookingAvailabilityConflict) when (isInitialSeriesMaterialization)
            {
                await initialSeriesTransaction!.RollbackAsync(cancellationToken);
                repositoryFactory.ResetChangeTracker();
                await FinalizeInitialSeriesAvailabilityFailureAsync(
                    subscription,
                    recurringBooking,
                    cancellationToken);
                return true;
            }
        }

        if (initialSeriesTransaction is not null)
        {
            try
            {
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                await initialSeriesTransaction.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsSerializableConflict(exception))
            {
                await initialSeriesTransaction.RollbackAsync(cancellationToken);
                repositoryFactory.ResetChangeTracker();
                await FinalizeInitialSeriesAvailabilityFailureAsync(
                    subscription,
                    recurringBooking,
                    cancellationToken);
                return true;
            }

            foreach (var createdBooking in createdInitialBookings)
            {
                await cachedBookingService.UpdateByIdAsync(createdBooking.Id, cancellationToken);
            }
        }

        return !reconciliationPlan.HasMoreRequiredBookingDays;
    }

    private static bool IsSerializableConflict(DbUpdateException exception) =>
        exception.InnerException?.Message.Contains("40001", StringComparison.Ordinal) == true ||
        exception.Message.Contains("40001", StringComparison.Ordinal);

    private async Task FinalizeInitialSeriesAvailabilityFailureAsync(
        MarketplaceBookingSubscription subscription,
        RecurringBooking recurringBooking,
        CancellationToken cancellationToken)
    {
        var failure = await marketplaceBookingFailureService.FinalizeAsync(
            new MarketplaceBookingFailureFinalization(
                null,
                MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
                MarketplaceBookingFailureScopeConstants.InitialSeries,
                timeProvider.GetUtcNow(),
                null,
                recurringBooking.Id,
                subscription.Id,
                recurringBooking.StartDate,
                recurringBooking.EndDate,
                ResolveRequiredResourceIds(subscription),
                MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription,
                null,
                "The requested booking series could not be confirmed because capacity is no longer available for every required occurrence.",
                recurringBooking.CreatedByCustomer?.Id,
                []),
            cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
            Constants.MarketplaceBookingSubscriptionTopicName,
            subscription.Id,
            cancellationToken);
        logger.LogWarning(
            "Finalized initial marketplace booking series availability failure. FailureId={FailureId}, SubscriptionId={SubscriptionId}, RecurringBookingId={RecurringBookingId}",
            failure.Id,
            subscription.Id,
            recurringBooking.Id);
    }

    private async Task<IReadOnlyList<string>> ResolvePreferredResourceIdsAsync(
        MarketplaceBookingSubscription subscription,
        RecurringBooking recurringBooking,
        IReadOnlyList<Database.Entities.Booking> existingBookingsToRefresh,
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
            var previousBookings = await repositoryFactory.BookingRepository.GetByRecurringBookingIdUntrackedAsync(
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

    private static IReadOnlyList<string> ResolveRequiredResourceIds(MarketplaceBookingSubscription subscription) =>
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
            await EnsureInitialArrearsInvoiceStartedAsync(subscription, existingRecurringBooking, cycleStart, cancellationToken);

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
                Frequency = subscription.WeeklySelectedDays.Count == 0
                    ? BookingFrequencyConstants.Daily
                    : BookingFrequencyConstants.Weekly,
                Interval = 1,
                ByMonthDay = null,
                BySetPosition = null,
                ByWeekDays = subscription.WeeklySelectedDays.Select(item => item.ToString()).ToList(),
                EndType = RecurringBookingEndTypeConstants.UntilDate,
                StartDate = cycleStart,
                EndDate = cycleEnd,
                OccurrenceCount = null,
                SkippedDates = [],
                RequestedResources = subscription.RequestedResources,
                InvolvedCustomers = subscription.InvolvedCustomers,
                InvolvedOrganizations = subscription.InvolvedOrganizations,
                InvolvedTeams = subscription.InvolvedTeams,
                CreatedByCustomer = subscription.CreatedByCustomer ?? subscription.InvolvedCustomers.FirstOrDefault(),
                LastModifiedByCustomer = null,
                DeletedByCustomer = null,
                MarketplaceBookingSubscription = subscription,
                MarketplaceBooking = recurringMarketplaceBooking,
            });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await EnsureInitialArrearsInvoiceStartedAsync(subscription, recurringBooking, cycleStart, cancellationToken);

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

    private async Task EnsureInitialArrearsInvoiceStartedAsync(
        MarketplaceBookingSubscription subscription,
        RecurringBooking recurringBooking,
        DateTimeOffset cycleStart,
        CancellationToken cancellationToken)
    {
        recurringBooking = await EnsureRecurringBookingMarketplaceBookingLoadedAsync(recurringBooking, cancellationToken);
        var recurringMarketplaceBooking = recurringBooking.MarketplaceBooking;
        if (recurringMarketplaceBooking is null ||
            recurringMarketplaceBooking.IsPaymentRequired ||
            string.IsNullOrWhiteSpace(recurringMarketplaceBooking.BillingMode) ||
            recurringMarketplaceBooking.BillingMode.ToProductPricingBillingMode() != ProductPricingBillingMode.InArrears ||
            !string.IsNullOrWhiteSpace(recurringMarketplaceBooking.InvoiceNumber) ||
            cycleStart.UtcDateTime.Date != subscription.StartedAt.UtcDateTime.Date)
        {
            return;
        }

        await temporalService.StartWorkflowGenerateInitialArrearsRecurringBookingInvoiceAsync(
            new GenerateInitialArrearsRecurringBookingInvoiceInput(
                recurringBooking.Id,
                recurringMarketplaceBooking.InvoiceEmailList.ToList()),
            cancellationToken);
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
        var persistedSubscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdUntrackedAsync(
            subscription.Id,
            cancellationToken);
        // The adjustment loop may be holding an older tracked snapshot while an immediate-cancel
        // request is processed elsewhere. If the current persisted row is already terminal, prefer
        // that state and skip any "active subscription" renewal logic so we do not accidentally
        // revive a cancelled subscription with stale data.
        if (persistedSubscription is not null &&
            persistedSubscription.Status is MarketplaceBookingSubscriptionStatusConstants.Cancelled
                or MarketplaceBookingSubscriptionStatusConstants.Expired
                or MarketplaceBookingSubscriptionStatusConstants.RenewalFailed
                or MarketplaceBookingSubscriptionStatusConstants.Paused)
        {
            return persistedSubscription;
        }

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
            await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingSubscriptionAsync(
                subscription, null, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

            return subscription;
        }

        while (subscription.NextRenewalAt.HasValue && subscription.NextRenewalAt.Value <= now)
        {
            if (!subscription.AutoRenew || !subscription.MarketplaceBooking.ProductPricing.SupportsSubscriptionAutoRenewal)
            {
                var terminalStatus = subscription.CancelAtPeriodEnd
                    ? MarketplaceBookingSubscriptionStatus.Cancelled
                    : MarketplaceBookingSubscriptionStatus.Expired;

                if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() != terminalStatus)
                {
                    subscription.Status = terminalStatus.ToMarketplaceBookingSubscriptionStatus();
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
                productVersionHelperService.FindMatchingPricing(productVersion.PricingOptions.ToList(),
                    subscription.MarketplaceBooking.ProductPricing);
            if (renewedProductPricing is null || !renewedProductPricing.SupportsSubscriptionAutoRenewal)
            {
                if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() != MarketplaceBookingSubscriptionStatus.RenewalFailed)
                {
                    subscription.Status = MarketplaceBookingSubscriptionStatus.RenewalFailed.ToMarketplaceBookingSubscriptionStatus();
                    hasChanges = true;
                }

                break;
            }

            try
            {
                var selectedDays = subscription.WeeklySelectedDays.Select(item => item.ToDayOfWeek()).ToList();
                // A subscription that already has a fixed weekly pattern must retain it even
                // if a later product version removes the optional weekly rule. Validate that
                // retained pattern against the renewed price's available-day pool rather than
                // letting the no-rule compatibility branch clear it and widen recurrence to daily.
                var validationPricing = renewedProductPricing.RequiredDaysPerWeek is null && selectedDays.Count > 0
                    ? renewedProductPricing with
                    {
                        RequiredDaysPerWeek = selectedDays.Count,
                    }
                    : renewedProductPricing;
                subscription.WeeklySelectedDays = marketplaceBookingWeeklyDaySelectionService.Validate(
                        validationPricing,
                        selectedDays)
                    .Select(item => item.ToDayOfWeek())
                    .ToList();
            }
            catch (MarketplaceBookingWeeklyDaySelectionInvalid exception)
            {
                logger.LogWarning(
                    exception,
                    "Rejected marketplace subscription renewal because its weekly selected days no longer match the current price. SubscriptionId: {SubscriptionId}, PricingId: {PricingId}, WeeklySelectedDays: {WeeklySelectedDays}",
                    subscription.Id,
                    renewedProductPricing.Id,
                    subscription.WeeklySelectedDays);
                if (subscription.Status.ToMarketplaceBookingSubscriptionStatus() != MarketplaceBookingSubscriptionStatus.RenewalFailed)
                {
                    subscription.Status = MarketplaceBookingSubscriptionStatus.RenewalFailed.ToMarketplaceBookingSubscriptionStatus();
                    hasChanges = true;
                }

                break;
            }

            logger.LogInformation(
                "Selected current price rule for marketplace subscription renewal. SubscriptionId: {SubscriptionId}, PreviousPricingId: {PreviousPricingId}, RenewedPricingId: {RenewedPricingId}, AvailableDays: {AvailableDays}",
                subscription.Id,
                subscription.MarketplaceBooking.ProductPricing.Id,
                renewedProductPricing.Id,
                renewedProductPricing.AvailableDays);
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
        await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingSubscriptionAsync(
            subscription, null, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return subscription;
    }

    private MarketplaceBooking CreateRecurringMarketplaceBookingTemplate(MarketplaceBookingSubscription subscription)
    {
        var marketplaceBooking = subscription.MarketplaceBooking;
        var bookingCadence = ResolveInstanceBookingCadence(marketplaceBooking.ProductPricing.PurchaseCadence);
        var requiresPaymentForCurrentCycle =
            marketplaceBooking.ProductPricing.BillingMode is ProductPricingBillingMode.Upfront or ProductPricingBillingMode.InArrears;
        var paymentExpiry = requiresPaymentForCurrentCycle
            ? timeProvider.GetUtcNow()
                .TrimAllAfterSeconds()
                .AddMinutes(GetBookingPaymentExpiryInMinutes(marketplaceBooking.ProductPricing, marketplaceBooking.PaymentMethod.ToPaymentMethod()))
            : default;

        return repositoryFactory.MarketplaceBookingRepository.Add(
            new MarketplaceBooking
            {
                Id = randomHelper.Generate(),
                // Every subscription cycle starts unpaid. Upfront cycles charge the full
                // cadence now, while in-arrears cycles charge only the first billing slice now.
                PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
                IsPaymentRequired = requiresPaymentForCurrentCycle,
                Quantity = marketplaceBooking.Quantity,
                ProductPricing = marketplaceBooking.ProductPricing with
                {
                    BookingCadence = bookingCadence,
                },
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
                StripeCheckoutSession = null,
            });
    }

    private static bool ShouldStartRecurringBookingCardPaymentWorkflow(MarketplaceBooking marketplaceBooking) =>
        marketplaceBooking.IsPaymentRequired &&
        !ShouldSkipResourceMaterializationForTerminalPaymentStatus(marketplaceBooking) &&
        marketplaceBooking.PaymentMethod.ToPaymentMethod() == PaymentMethod.Card;

    private static bool ShouldStartRecurringBookingBankTransferPaymentWorkflow(MarketplaceBooking marketplaceBooking) =>
        marketplaceBooking.IsPaymentRequired &&
        !ShouldSkipResourceMaterializationForTerminalPaymentStatus(marketplaceBooking) &&
        marketplaceBooking.PaymentMethod.ToPaymentMethod() == PaymentMethod.BankTransfer;

    private static bool ShouldSkipResourceMaterializationForTerminalPaymentStatus(MarketplaceBooking? marketplaceBooking) =>
        marketplaceBooking is not null &&
        marketplaceBooking.IsPaymentRequired &&
        marketplaceBooking.PaymentStatus.ToPaymentStatus() is PaymentStatus.Expired or PaymentStatus.Rejected or PaymentStatus.RecordNeverCreated;

    private async Task MarkRecurringBookingPaymentAsTerminalAsync(
        RecurringBooking recurringBooking,
        CancellationToken cancellationToken)
    {
        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        if (marketplaceBooking is null || !marketplaceBooking.IsPaymentRequired)
        {
            return;
        }

        marketplaceBooking.PaymentStatus = string.IsNullOrWhiteSpace(marketplaceBooking.InvoiceUrl) &&
                                           marketplaceBooking.StripeCheckoutSession is null
            ? PaymentStatusConstants.RecordNeverCreated
            : PaymentStatusConstants.Expired;
        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);
    }

    private async Task MarkSubscriptionPaymentAsTerminalAsync(
        RecurringBooking recurringBooking, string failureCategory, CancellationToken cancellationToken)
    {
        var subscription = recurringBooking.MarketplaceBookingSubscription;
        if (subscription is null || subscription.Status.ToMarketplaceBookingSubscriptionStatus() is
                MarketplaceBookingSubscriptionStatus.Cancelled or
                MarketplaceBookingSubscriptionStatus.Expired or
                MarketplaceBookingSubscriptionStatus.RenewalFailed or
                MarketplaceBookingSubscriptionStatus.Paused)
        {
            return;
        }

        subscription.Status = failureCategory == MarketplaceBookingFailureCategoryConstants.PaymentExpired
            ? MarketplaceBookingSubscriptionStatus.Expired.ToMarketplaceBookingSubscriptionStatus()
            : MarketplaceBookingSubscriptionStatus.RenewalFailed.ToMarketplaceBookingSubscriptionStatus();
        subscription.AutoRenew = false;
        subscription.NextRenewalAt = null;
        repositoryFactory.MarketplaceBookingSubscriptionRepository.Update(subscription);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingSubscriptionAsync(
            subscription, null, cancellationToken);
    }

    private static int GetBookingPaymentExpiryInMinutes(ProductPricing pricing, PaymentMethod paymentMethod) =>
        paymentMethod switch
        {
            PaymentMethod.Card => pricing.MaxAllowedResourcesLockTimePaidViaCard,
            PaymentMethod.BankTransfer => pricing.MaxAllowedResourcesLockTimePaidViaBankTransfer,
            _ => throw new ArgumentOutOfRangeException(nameof(paymentMethod), paymentMethod,
                $"Unexpected value for {nameof(paymentMethod)}: {paymentMethod}. Update enum mapping or caller input."),
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

        if (subscription is { AutoRenew: true, MarketplaceBooking.ProductPricing.SupportsSubscriptionAutoRenewal: true })
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
            _ => cycleEndExclusive.AddDays(-1),
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
            _ => start.AddDays(1),
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
            _ => new DateTimeOffset(from.UtcDateTime.Date.AddDays(1).AddTicks(-1), TimeSpan.Zero),
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
            _ => ProductPricingCadence.Daily,
        };
}
