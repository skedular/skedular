using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Constants = Booking.Shared.GraphQL.Constants;
using Customer = Booking.Shared.Database.Entities.Customer;
using MarketplaceBooking = Booking.Shared.Models.MarketplaceBooking;
using MarketplaceBookingSubscription = Booking.Shared.Models.MarketplaceBookingSubscription;
using Organization = Booking.Shared.Database.Entities.Organization;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Shared.Services;

public interface IMarketplaceBookingSubscriptionService
{
    Task<MarketplaceBookingSubscription> AddAsync(
        MarketplaceBookingSubscription subscription,
        Customer customer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken);

    Task<MarketplaceBookingSubscription> DeleteAsync(
        Database.Entities.MarketplaceBookingSubscription existingSubscription,
        Customer? deletedByCustomer,
        MarketplaceBookingSubscriptionCancellationMode cancellationMode,
        bool ignoreCancellationPolicy,
        string? cancellationOverrideReason,
        CancellationToken cancellationToken);
}

public class MarketplaceBookingSubscriptionService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IEntityMapper entityMapper,
    IProductVersionHelperService productVersionHelperService,
    IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
    ITemporalOutboxService temporalOutboxService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    MarketplaceRefundPolicyService marketplaceRefundPolicyService,
    IMarketplaceRefundService marketplaceRefundService,
    ISpacesBookingQuotaService spacesBookingQuotaService,
    IMarketplaceBookingAvailableDaysService marketplaceBookingAvailableDaysService,
    IMarketplaceBookingWeeklyDaySelectionService marketplaceBookingWeeklyDaySelectionService,
    ILogger<MarketplaceBookingSubscriptionService> logger) : IMarketplaceBookingSubscriptionService
{
    public async Task<MarketplaceBookingSubscription> AddAsync(
        MarketplaceBookingSubscription subscription,
        Customer customer,
        IReadOnlyList<Organization> organizations,
        IReadOnlyList<Team> teams,
        CancellationToken cancellationToken)
    {
        var customerIds = subscription.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var requestedResourceIds = subscription.RequestedResources.Select(item => item.Id).Distinct().ToList();
        var resourceEntities = requestedResourceIds.Count == 0
            ? []
            : await repositoryFactory.ResourceRepository.GetByIdsAsync(requestedResourceIds, false, cancellationToken);
        if (resourceEntities.Count != requestedResourceIds.Count)
        {
            throw new ResourceNotFound();
        }

        var marketplaceBooking = subscription.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.OrganizationTags.All(item => item.Type != OrganizationTagTypeConstants.Product))
        {
            throw new ProductMissingProductTag();
        }

        if (productVersion.Type == ProductTypeConstants.Event)
        {
            throw new MarketplaceEventProductRecurringBookingNotSupported();
        }

        ArgumentNullException.ThrowIfNull(productVersion.PricingOptions);

        marketplaceBooking.ProductPricing =
            productVersionHelperService.FindMatchingPricing(productVersion.PricingOptions.ToList(), marketplaceBooking.ProductPricing) ??
            throw new ProductPricingNotFound();
        try
        {
            subscription.WeeklySelectedDays = marketplaceBookingWeeklyDaySelectionService.Validate(
                marketplaceBooking.ProductPricing,
                subscription.WeeklySelectedDays);
        }
        catch (MarketplaceBookingWeeklyDaySelectionInvalid exception)
        {
            logger.LogWarning(
                exception,
                "Rejected marketplace subscription weekly-day selection. ProductVersionId: {ProductVersionId}, PricingId: {PricingId}, StartedAt: {StartedAt}, WeeklySelectedDays: {WeeklySelectedDays}",
                productVersion.Id,
                marketplaceBooking.ProductPricing.Id,
                subscription.StartedAt,
                subscription.WeeklySelectedDays);
            throw;
        }

        marketplaceBooking.Currency ??= productVersion.Currency.ToNullableCurrency();
        // A fixed weekly selection owns the booking dates. Its membership in the price's
        // available-day pool was validated above, so the arbitrary subscription start date
        // must not reject checkout or preflight a requested resource on an unselected day.
        // Reconciliation will evaluate each selected date and create a shell when it cannot
        // assign the requested/compatible resources on that date.
        var hasFixedWeeklySelection = MarketplaceBookingWeeklyDaySelectionService.UsesFixedWeeklySchedule(
            marketplaceBooking.ProductPricing,
            subscription.WeeklySelectedDays);
        switch (hasFixedWeeklySelection)
        {
            case false when !marketplaceBookingAvailableDaysService.IsAvailable(
                marketplaceBooking.ProductPricing,
                subscription.StartedAt,
                out var localDate):
                logger.LogWarning(
                    "Rejected marketplace subscription for unavailable price day. ProductVersionId: {ProductVersionId}, PricingId: {PricingId}, StartedAt: {StartedAt}, LocalDate: {LocalDate}",
                    productVersion.Id,
                    marketplaceBooking.ProductPricing.Id,
                    subscription.StartedAt,
                    localDate);
                throw new MarketplaceBookingDateUnavailable();

            case false:
                await EnsureRequestedResourceCanBeBookedAsync(
                    subscription,
                    productVersion,
                    marketplaceBooking,
                    marketplaceBookingOpeningHoursService,
                    cancellationToken);
                break;
        }

        // Subscription checkout also happens asynchronously later in Temporal, so the initial
        // marketplace-booking template must carry the storefront URL that Stripe should return
        // the customer to after hosted checkout finishes or is cancelled.
        marketplaceBooking.CheckoutReturnUrl = NormalizeCheckoutReturnUrl(marketplaceBooking.CheckoutReturnUrl);
        // Subscriptions should be manageable by the coworking-space owner organization too,
        // so the product owner's organization is always merged into involved organizations.
        organizations = MergeOrganizationsWithProductOwner(organizations, productVersion);
        foreach (var organization in organizations
                     .Where(item => item.Type == OrganizationTypeConstants.Marketplace)
                     .DistinctBy(item => item.Id))
        {
            var decision = await spacesBookingQuotaService.EvaluateAccessAsync(
                organization.Id,
                SpacesAccessAction.CreateOrModify,
                cancellationToken);
            if (!decision.Allowed)
            {
                throw new SpacesAccessDenied(decision);
            }
        }

        if (subscription.AutoRenew && !marketplaceBooking.ProductPricing.SupportsSubscriptionAutoRenewal)
        {
            throw new MarketplaceBookingSubscriptionAutoRenewalNotSupported();
        }

        marketplaceBooking.Id = randomHelper.Generate();
        marketplaceBooking.BillingMode = marketplaceBooking.ProductPricing.BillingMode;
        marketplaceBooking.IsPaymentRequired = true;
        marketplaceBooking.PaymentStatus = PaymentStatus.NotSet;

        if (!marketplaceBooking.ProductPricing.AcceptedPaymentMethods.Contains(marketplaceBooking.PaymentMethod))
        {
            throw new BookingPaymentMethodNotAccepted();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var marketplaceBookingEntity = repositoryFactory.MarketplaceBookingRepository.Add(entityMapper.MapTo(
            marketplaceBooking,
            customer,
            null,
            productVersion,
            null));

        var subscriptionEntity = repositoryFactory.MarketplaceBookingSubscriptionRepository.Add(entityMapper.MapTo(
            subscription,
            customerEntities,
            organizations,
            teams,
            resourceEntities,
            customer,
            null,
            null,
            marketplaceBookingEntity,
            productVersion));

        subscription = entityMapper.MapTo(subscriptionEntity);

        temporalOutboxService.StartBookMarketplaceBookingSubscriptionResources(
            new BookMarketplaceBookingSubscriptionResourcesInput(subscription.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingSubscriptionAsync(
            subscriptionEntity, null, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return subscription;
    }

    public async Task<MarketplaceBookingSubscription> DeleteAsync(
        Database.Entities.MarketplaceBookingSubscription existingSubscription,
        Customer? deletedByCustomer,
        MarketplaceBookingSubscriptionCancellationMode cancellationMode,
        bool ignoreCancellationPolicy,
        string? cancellationOverrideReason,
        CancellationToken cancellationToken) =>
        await DeleteAsync(existingSubscription, deletedByCustomer, cancellationMode, ignoreCancellationPolicy, cancellationOverrideReason,
            cancellationToken, 0);

    private async Task<MarketplaceBookingSubscription> DeleteAsync(
        Database.Entities.MarketplaceBookingSubscription existingSubscription,
        Customer? deletedByCustomer,
        MarketplaceBookingSubscriptionCancellationMode cancellationMode,
        bool ignoreCancellationPolicy,
        string? cancellationOverrideReason,
        CancellationToken cancellationToken,
        int concurrencyRetryCount,
        bool cancellationPolicyOverriddenFromInitialAttempt = false)
    {
        var cancellationPolicyOverriddenForRetry = cancellationPolicyOverriddenFromInitialAttempt;
        try
        {
            logger.LogInformation(
                "Marketplace subscription cancellation started. SubscriptionId={SubscriptionId}; cancellationMode={CancellationMode}; requestedByCustomerId={CustomerId}",
                existingSubscription.Id,
                cancellationMode,
                deletedByCustomer?.Id);
            var subscriptionId = existingSubscription.Id;

            // Validate the request object only on the first attempt. A failed attempt mutates this
            // tracked instance before SaveChanges rolls back, so retry must rely solely on the fresh
            // database reload below rather than treating those uncommitted mutations as terminal.
            var cancellationPolicyOverridden = cancellationPolicyOverriddenFromInitialAttempt;
            var requestedCancellationOverrideReason = cancellationOverrideReason;
            if (concurrencyRetryCount == 0)
            {
                if (cancellationMode == MarketplaceBookingSubscriptionCancellationMode.Immediate &&
                    existingSubscription.Status == MarketplaceBookingSubscriptionStatus.Cancelled.ToMarketplaceBookingSubscriptionStatus() &&
                    existingSubscription.CancelledAt.HasValue)
                {
                    return entityMapper.MapTo(existingSubscription);
                }

                if (deletedByCustomer is not null)
                {
                    try
                    {
                        EnsureSubscriptionCanStillBeCancelled(existingSubscription, cancellationMode);
                    }
                    catch (MarketplaceBookingSubscriptionCancellationNotAllowed) when (
                        ignoreCancellationPolicy && !string.IsNullOrWhiteSpace(cancellationOverrideReason))
                    {
                        cancellationPolicyOverridden = true;
                        cancellationPolicyOverriddenForRetry = true;
                        logger.LogInformation(
                            "Marketplace subscription cancellation policy overridden by authorized operator. SubscriptionId={SubscriptionId}",
                            existingSubscription.Id);
                    }
                    catch (MarketplaceBookingSubscriptionCancellationNotAllowed) when (ignoreCancellationPolicy)
                    {
                        throw new MarketplaceBookingSubscriptionCancellationOverrideReasonRequired();
                    }
                }
            }

            await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

            // The GraphQL/API layer may have loaded this aggregate before a payment webhook or
            // subscription workflow updated it. Reload it inside the cancellation transaction so
            // the delete/refund decision uses the current xmin concurrency token.
            repositoryFactory.ResetChangeTracker();
            existingSubscription = await repositoryFactory.MarketplaceBookingSubscriptionRepository.GetByIdForUpdateAsync(
                                       subscriptionId,
                                       cancellationToken) ??
                                   throw new InvalidOperationException($"Marketplace booking subscription was not found: {subscriptionId}");
            cancellationPolicyOverridden |= existingSubscription.CancellationPolicyOverridden;
            cancellationPolicyOverriddenForRetry |= cancellationPolicyOverridden;
            requestedCancellationOverrideReason ??= existingSubscription.CancellationOverrideReason;
            var trackedDeletedByCustomer = deletedByCustomer is null
                ? null
                : await repositoryFactory.CustomerRepository.GetByIdAsync(
                    deletedByCustomer.Id,
                    true,
                    cancellationToken);

            // Cleanup signals are replayed by Temporal. Once an immediate cancellation has
            // committed, return the existing aggregate without creating another refund boundary
            // or sending another provider-cancellation signal.
            if (cancellationMode == MarketplaceBookingSubscriptionCancellationMode.Immediate &&
                existingSubscription.Status == MarketplaceBookingSubscriptionStatus.Cancelled.ToMarketplaceBookingSubscriptionStatus() &&
                existingSubscription.CancelledAt.HasValue)
            {
                return entityMapper.MapTo(existingSubscription);
            }

            if (cancellationMode == MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd)
            {
                existingSubscription.CancellationPolicyOverridden = cancellationPolicyOverridden;
                existingSubscription.CancellationOverrideReason = cancellationPolicyOverridden ? requestedCancellationOverrideReason : null;
                existingSubscription.LastModifiedByCustomer = trackedDeletedByCustomer;
                existingSubscription.CancelledAt = timeProvider.GetUtcNow();
                existingSubscription.CancelAtPeriodEnd = true;
                existingSubscription.AutoRenew = false;
                existingSubscription.NextRenewalAt ??= ResolveNextRenewalAt(
                    existingSubscription.StartedAt,
                    existingSubscription.MarketplaceBooking.ProductPricing.PurchaseCadence);
                existingSubscription.ModifiedAt = timeProvider.GetUtcNow();
            }
            else
            {
                existingSubscription.CancellationPolicyOverridden = cancellationPolicyOverridden;
                existingSubscription.CancellationOverrideReason = cancellationPolicyOverridden ? requestedCancellationOverrideReason : null;
                existingSubscription.LastModifiedByCustomer = trackedDeletedByCustomer;
                existingSubscription.CancelledAt = timeProvider.GetUtcNow();
                existingSubscription.Status = MarketplaceBookingSubscriptionStatus.Cancelled.ToMarketplaceBookingSubscriptionStatus();
                existingSubscription.AutoRenew = false;
                existingSubscription.CancelAtPeriodEnd = false;
                existingSubscription.ModifiedAt = timeProvider.GetUtcNow();

                var refund = await marketplaceRefundService.CreateImmediateSubscriptionCancellationRefundAsync(
                    existingSubscription,
                    deletedByCustomer,
                    cancellationToken);

                logger.LogInformation(
                    "Marketplace subscription cancellation refund result. SubscriptionId={SubscriptionId}; refundId={RefundId}; refundStatus={RefundStatus}",
                    existingSubscription.Id,
                    refund?.Id,
                    refund?.Status);

                temporalOutboxService.SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(
                    existingSubscription.Id,
                    repositoryFactory.UnitOfWork);

                await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingSubscriptionAsync(
                    existingSubscription, refund, cancellationToken);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                if (refund is not null)
                {
                    await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.MarketplaceBookingSubscriptionTopicName,
                        existingSubscription.Id,
                        cancellationToken);
                }

                return entityMapper.MapTo(existingSubscription);
            }

            await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingSubscriptionAsync(
                existingSubscription, null, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return entityMapper.MapTo(existingSubscription);
        }
        catch (DbUpdateConcurrencyException exception) when (concurrencyRetryCount < 2)
        {
            logger.LogWarning(
                exception,
                "Marketplace booking subscription cancellation conflicted with a concurrent update. Retrying. SubscriptionId: {SubscriptionId}, RetryCount: {RetryCount}",
                existingSubscription.Id,
                concurrencyRetryCount + 1);
            repositoryFactory.ResetChangeTracker();
            return await DeleteAsync(
                existingSubscription,
                deletedByCustomer,
                cancellationMode,
                ignoreCancellationPolicy,
                cancellationOverrideReason,
                cancellationToken,
                concurrencyRetryCount + 1,
                cancellationPolicyOverriddenForRetry);
        }
    }

    private static List<Organization> MergeOrganizationsWithProductOwner(
        IReadOnlyList<Organization> organizations,
        ProductVersion productVersion)
    {
        ArgumentNullException.ThrowIfNull(productVersion.Product);
        ArgumentNullException.ThrowIfNull(productVersion.Product.Organization);

        return
        [
            .. organizations
                .Append(productVersion.Product.Organization)
                .GroupBy(item => item.Id)
                .Select(item => item.First()),
        ];
    }

    private static string? NormalizeCheckoutReturnUrl(string? checkoutReturnUrl)
    {
        if (string.IsNullOrWhiteSpace(checkoutReturnUrl))
        {
            return null;
        }

        if (!Uri.TryCreate(checkoutReturnUrl, UriKind.Absolute, out var returnUri) ||
            (returnUri.Scheme != Uri.UriSchemeHttps && returnUri.Scheme != Uri.UriSchemeHttp))
        {
            throw new MarketplaceBookingCheckoutReturnUrlInvalid();
        }

        return returnUri.ToString();
    }

    private static async Task EnsureRequestedResourceCanBeBookedAsync(
        MarketplaceBookingSubscription subscription,
        ProductVersion productVersion,
        MarketplaceBooking marketplaceBooking,
        IMarketplaceBookingOpeningHoursService marketplaceBookingOpeningHoursService,
        CancellationToken cancellationToken)
    {
        var requestedResourceIds = subscription.RequestedResources.Select(item => item.Id).Distinct().ToList();
        if (requestedResourceIds.Count == 0)
        {
            return;
        }

        var bookingDay = DateOnly.FromDateTime(subscription.StartedAt.UtcDateTime.Date);
        var requiredResourceCount = marketplaceBooking.Quantity * marketplaceBooking.ProductPricing.NumberOfResourcesToBook;
        var dailyPlan = await marketplaceBookingOpeningHoursService.TryResolveDailyPlanAsync(
            null,
            productVersion,
            marketplaceBooking.ProductPricing,
            bookingDay,
            requiredResourceCount,
            requestedResourceIds,
            [],
            null,
            cancellationToken);
        if (dailyPlan is null || dailyPlan.Resources.Count != requiredResourceCount)
        {
            throw new ResourceNotAvailable();
        }
    }

    private void EnsureSubscriptionCanStillBeCancelled(
        Database.Entities.MarketplaceBookingSubscription existingSubscription,
        MarketplaceBookingSubscriptionCancellationMode cancellationMode)
    {
        var marketplaceBooking = existingSubscription.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        if (cancellationMode == MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd)
        {
            if (!existingSubscription.AutoRenew)
            {
                throw new MarketplaceBookingSubscriptionCancellationNotAllowed();
            }

            return;
        }

        var referenceTime = existingSubscription.NextRenewalAt ?? existingSubscription.StartedAt;
        var quote = marketplaceRefundPolicyService.GetQuote(marketplaceBooking.ProductPricing, referenceTime, timeProvider.GetUtcNow());
        if (!quote.CanCancel)
        {
            throw new MarketplaceBookingSubscriptionCancellationNotAllowed();
        }
    }

    private static DateTimeOffset ResolveNextRenewalAt(DateTimeOffset startedAt, ProductPricingCadence cadence) =>
        cadence switch
        {
            ProductPricingCadence.Daily => startedAt.AddDays(1),
            ProductPricingCadence.Weekly => startedAt.AddDays(7),
            ProductPricingCadence.Fortnightly => startedAt.AddDays(14),
            ProductPricingCadence.Monthly => startedAt.AddMonths(1),
            ProductPricingCadence.TwoMonths => startedAt.AddMonths(2),
            ProductPricingCadence.Quarterly => startedAt.AddMonths(3),
            ProductPricingCadence.FourMonths => startedAt.AddMonths(4),
            ProductPricingCadence.FiveMonths => startedAt.AddMonths(5),
            ProductPricingCadence.SixMonths => startedAt.AddMonths(6),
            ProductPricingCadence.Yearly => startedAt.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(cadence), cadence,
                $"Unexpected value for {nameof(cadence)}: {cadence}. Update enum mapping or caller input."),
        };
}
