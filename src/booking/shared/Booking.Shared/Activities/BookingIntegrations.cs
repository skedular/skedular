using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Workflows;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;
using Microsoft.Extensions.Logging;
using Temporalio.Activities;
using Constants = Booking.Shared.GraphQL.Constants;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using MarketplaceBookingFailureCategoryConstants = Booking.Shared.Models.MarketplaceBookingFailureCategoryConstants;
using MarketplaceBookingFailureCustomerActionConstants = Booking.Shared.Models.MarketplaceBookingFailureCustomerActionConstants;
using MarketplaceBookingFailureFinalization = Booking.Shared.Models.MarketplaceBookingFailureFinalization;
using MarketplaceBookingFailureScopeConstants = Booking.Shared.Models.MarketplaceBookingFailureScopeConstants;
using MarketplaceBookingFailureAccountingCleanupStatus = Booking.Shared.Models.MarketplaceBookingFailureAccountingCleanupStatus;

namespace Booking.Shared.Activities;

public record ReleaseBookingResourcesInput(string BookingId, string? FailureCategory = null);

public record CalculateBookingDifferentAmountsInput(string BookingId);

public record CalculateRecurringBookingDifferentAmountsInput(string RecurringBookingId);

public class BookingIntegrations(
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    IEntityMapper entityMapper,
    IOrganizationArrearsBillingPlannerService organizationArrearsBillingPlannerService,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ICachedBookingService cachedBookingService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IAccountingInvoiceCancellationService accountingInvoiceCancellationService,
    IMarketplaceBookingFailureService marketplaceBookingFailureService,
    IHostCommissionService hostCommissionService,
    IMarketplaceRefundService marketplaceRefundService,
    ITemporalOutboxService temporalOutboxService,
    TimeProvider timeProvider,
    ILogger<BookingIntegrations> logger)
{
    [Activity]
    public async Task CalculateBookingDifferentAmountsAsync(CalculateBookingDifferentAmountsInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted())
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);

        var marketplaceBooking = await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(booking.MarketplaceBooking.Id, cancellationToken);
        if (marketplaceBooking is null || booking.IsDeleted())
        {
            return;
        }

        var organization = await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput
            {
                Id = marketplaceBooking.ProductVersion.Product.Organization.Id,
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(organization);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var totalPrice = marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity;

        marketplaceBooking.Currency = marketplaceBooking.ProductVersion.Currency;

        if (!IsRegisteredForTax(organization.TaxDetails))
        {
            marketplaceBooking.TotalAmountExcludeTax = totalPrice;
            marketplaceBooking.TaxAmount = 0.00m;
            marketplaceBooking.TaxRatePercentage = 0.00m;
            marketplaceBooking.TotalAmount = totalPrice.RoundedDecimal();
        }
        else
        {
            var isPriceTaxInclusive = marketplaceBooking.ProductPricing.IsTaxInclusive;
            var taxRatePercentage = Convert.ToDecimal(organization.TaxDetails.TaxRatePercentage);
            marketplaceBooking.TaxRatePercentage = taxRatePercentage.RoundedDecimal();

            if (isPriceTaxInclusive)
            {
                marketplaceBooking.TotalAmount = totalPrice.RoundedDecimal();
                marketplaceBooking.TotalAmountExcludeTax = (marketplaceBooking.TotalAmount.Value * 100 / (100 + taxRatePercentage)).RoundedDecimal();
                marketplaceBooking.TaxAmount =
                    (marketplaceBooking.TotalAmount.Value - marketplaceBooking.TotalAmountExcludeTax.Value).RoundedDecimal();
            }
            else
            {
                marketplaceBooking.TotalAmountExcludeTax = totalPrice.RoundedDecimal();
                marketplaceBooking.TaxAmount = (marketplaceBooking.TotalAmountExcludeTax.Value * taxRatePercentage / 100).RoundedDecimal();
                marketplaceBooking.TotalAmount =
                    (marketplaceBooking.TotalAmountExcludeTax.Value + marketplaceBooking.TaxAmount.Value).RoundedDecimal();
            }
        }

        var commissionRate = organization.Offering.Code.ToOfferingCode().GetEffectiveHostCommissionPercentage(
            Convert.ToDecimal(organization.Offering.HostCommissionPercentage));
        ApplyHostCommission(marketplaceBooking, commissionRate);

        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        bookingOutboxPublisher.PublishBookings([entityMapper.MapTo(booking)], repositoryFactory.UnitOfWork);

        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
    }

    [Activity]
    public async Task CalculateRecurringBookingDifferentAmountsAsync(CalculateRecurringBookingDifferentAmountsInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted() || recurringBooking.MarketplaceBooking is null)
        {
            return;
        }

        var marketplaceBooking =
            await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(recurringBooking.MarketplaceBooking.Id, cancellationToken);
        if (marketplaceBooking is null)
        {
            return;
        }

        var organization = await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput
            {
                Id = marketplaceBooking.ProductVersion.Product.Organization.Id,
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(organization);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var totalPrice = (marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity).RoundedDecimal();
        marketplaceBooking.Currency = marketplaceBooking.ProductVersion.Currency;

        if (marketplaceBooking.BillingMode.ToProductPricingBillingMode() == ProductPricingBillingMode.InArrears)
        {
            var recurringBookingModel = entityMapper.MapTo(recurringBooking);
            var draft = organizationArrearsBillingPlannerService.BuildInitialRecurringInvoiceDraft(
                recurringBookingModel,
                marketplaceBooking.ProductVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());
            if (draft is not null)
            {
                totalPrice = draft.TotalAmount.RoundedDecimal();
            }
        }

        if (!IsRegisteredForTax(organization.TaxDetails))
        {
            marketplaceBooking.TotalAmountExcludeTax = totalPrice;
            marketplaceBooking.TaxAmount = 0.00m;
            marketplaceBooking.TaxRatePercentage = 0.00m;
            marketplaceBooking.TotalAmount = totalPrice;
        }

        else
        {
            var isPriceTaxInclusive = marketplaceBooking.ProductPricing.IsTaxInclusive;
            var taxRatePercentage = Convert.ToDecimal(organization.TaxDetails.TaxRatePercentage);
            marketplaceBooking.TaxRatePercentage = taxRatePercentage.RoundedDecimal();

            if (isPriceTaxInclusive)
            {
                marketplaceBooking.TotalAmount = totalPrice;
                marketplaceBooking.TotalAmountExcludeTax = (marketplaceBooking.TotalAmount.Value * 100 / (100 + taxRatePercentage)).RoundedDecimal();
                marketplaceBooking.TaxAmount =
                    (marketplaceBooking.TotalAmount.Value - marketplaceBooking.TotalAmountExcludeTax.Value).RoundedDecimal();
            }
            else
            {
                marketplaceBooking.TotalAmountExcludeTax = totalPrice;
                marketplaceBooking.TaxAmount = (marketplaceBooking.TotalAmountExcludeTax.Value * taxRatePercentage / 100).RoundedDecimal();
                marketplaceBooking.TotalAmount =
                    (marketplaceBooking.TotalAmountExcludeTax.Value + marketplaceBooking.TaxAmount.Value).RoundedDecimal();
            }
        }

        var commissionRate = organization.Offering.Code.ToOfferingCode().GetEffectiveHostCommissionPercentage(
            Convert.ToDecimal(organization.Offering.HostCommissionPercentage));
        ApplyHostCommission(marketplaceBooking, commissionRate);

        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (recurringBooking.MarketplaceBookingSubscription is not null)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                recurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }
    }

    [Activity]
    public async Task ReleaseBookingResourcesAsync(ReleaseBookingResourcesInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted() || booking.MarketplaceBooking is null)
        {
            return;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var marketplaceBooking = booking.MarketplaceBooking;
        var failureCategory = args.FailureCategory ?? MarketplaceBookingFailureCategoryConstants.PaymentExpired;
        // Capture the payment outcome before terminal cleanup rewrites the local status.
        // Otherwise a confirmed payment is changed to Expired and the compensating refund
        // path is skipped.
        var wasAlreadyPaid = marketplaceBooking.PaymentStatus == PaymentStatusConstants.Confirmed;
        var failure = await marketplaceBookingFailureService.FinalizeAsync(
            new MarketplaceBookingFailureFinalization(
                null,
                failureCategory,
                MarketplaceBookingFailureScopeConstants.OneTimeBooking,
                timeProvider.GetUtcNow(),
                booking.Id,
                null,
                null,
                booking.From,
                booking.Until,
                [.. booking.InvolvedResources.Select(item => item.Id)],
                MarketplaceBookingFailureCustomerActionConstants.Rebook,
                null,
                "Payment was not completed before the booking resources were released.",
                booking.CreatedByCustomer?.Id,
                []),
            cancellationToken);

        MarketplaceRefund? refundToProcess = null;
        if (wasAlreadyPaid)
        {
            // The refund service deliberately requires confirmed payment, so create the durable
            // refund request before terminal cleanup changes the local payment status.
            refundToProcess = await marketplaceRefundService.CreateBookingCancellationRefundAsync(
                booking, null, cancellationToken, true);
        }

        marketplaceBooking.PaymentStatus = marketplaceBooking.StripeCheckoutSession is null
            ? PaymentStatusConstants.RecordNeverCreated
            : PaymentStatusConstants.Expired;

        booking.InvolvedResources.Clear();
        repositoryFactory.BookingRepository.Update(booking);
        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking);

        bookingOutboxPublisher.PublishBookings([entityMapper.MapTo(booking)], repositoryFactory.UnitOfWork);

        if (refundToProcess is not null)
        {
            temporalOutboxService.StartWorkflowProcessMarketplaceRefund(
                new ProcessMarketplaceRefundInput(refundToProcess.Id, null), repositoryFactory.UnitOfWork);
        }

        await marketplaceBookingFailureService.MarkResourcesReleasedAsync(
            failure.Id,
            MarketplaceBookingFailureAccountingCleanupStatus.Pending,
            cancellationToken);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.UpsertMarketplaceBookingAsync(
            booking, refundToProcess, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        try
        {
            await accountingInvoiceCancellationService.CancelBookingAsync(booking, cancellationToken);
            await marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatus.NotRequired,
                cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            await marketplaceBookingFailureService.MarkResourcesReleasedAsync(
                failure.Id,
                MarketplaceBookingFailureAccountingCleanupStatus.TransitionRequired,
                cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogWarning(
                exception,
                "Marketplace booking resources were released but accounting cancellation requires a transition. BookingId={BookingId}, FailureId={FailureId}",
                booking.Id,
                failure.Id);
        }

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
        logger.LogInformation(
            "Released marketplace booking resources after terminal payment outcome. BookingId={BookingId}, FailureCategory={FailureCategory}",
            booking.Id,
            failureCategory);
    }

    private static bool IsRegisteredForTax(TaxDetails? taxDetails) =>
        taxDetails is { IsRegistered: true };

    private void ApplyHostCommission(MarketplaceBooking marketplaceBooking, decimal commissionRatePercentage)
    {
        var result = hostCommissionService.Calculate(
            marketplaceBooking.ProductVersion.Product.Organization.Type,
            commissionRatePercentage,
            marketplaceBooking.TotalAmount ?? 0m);
        if (result is null)
        {
            marketplaceBooking.HostCommissionRatePercentage = null;
            marketplaceBooking.HostCommissionAmount = null;
            marketplaceBooking.HostPayoutAmount = null;
            return;
        }

        marketplaceBooking.HostCommissionRatePercentage = result.RatePercentage;
        marketplaceBooking.HostCommissionAmount = result.Amount;
        marketplaceBooking.HostPayoutAmount = result.HostPayoutAmount;
        logger.LogInformation(
            "Host commission calculated. BookingId: {BookingId}, MarketplaceBookingId: {MarketplaceBookingId}, RatePercentage: {RatePercentage}, CommissionAmount: {CommissionAmount}, HostPayoutAmount: {HostPayoutAmount}",
            marketplaceBooking.BookingId,
            marketplaceBooking.Id,
            result.RatePercentage,
            result.Amount,
            marketplaceBooking.HostPayoutAmount);
    }
}
