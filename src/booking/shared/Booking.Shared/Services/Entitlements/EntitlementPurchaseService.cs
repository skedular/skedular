using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GraphQlConstants = Booking.Shared.GraphQL.Constants;
using EntitlementPurchase = Booking.Shared.Models.Entitlements.EntitlementPurchase;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementPurchaseService
{
    Task<EntitlementPurchase?> GetByIdAsync(string purchaseId, CancellationToken cancellationToken);
    Task SetCheckoutReturnUrlAsync(string purchaseId, string checkoutReturnUrl, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitlementPurchase>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitlementPurchase>> GetForOrganizationAsync(string organizationId, CancellationToken cancellationToken);
    Task<EntitlementPurchaseProductModel?> GetProductAsync(string productVersionId, string pricingId, CancellationToken cancellationToken);

    Task<EntitlementPurchase> CreatePendingAsync(
        string customerId,
        string organizationId,
        string productVersionId,
        ProductPricing pricing,
        string currency,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentExpiry,
        string? checkoutReturnUrl,
        IReadOnlyCollection<string> invoiceEmailList,
        CancellationToken cancellationToken);

    Task<EntitlementPurchase> CreatePendingAsync(
        string customerId, string organizationId, string productVersionId, ProductPricing pricing, string currency,
        PaymentMethod paymentMethod, DateTimeOffset paymentExpiry, DateTimeOffset serviceStartAt, string? checkoutReturnUrl,
        IReadOnlyCollection<string> invoiceEmailList, bool autoRenew, CancellationToken cancellationToken);

    Task<EntitlementPurchase> CreatePendingAsync(
        string customerId,
        string organizationId,
        string productVersionId,
        ProductPricing pricing,
        string currency,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentExpiry,
        DateTimeOffset serviceStartAt,
        string? checkoutReturnUrl,
        IReadOnlyCollection<string> invoiceEmailList,
        CancellationToken cancellationToken);

    Task<EntitlementModel> ConfirmAsync(string purchaseId, DateTimeOffset activatesAt, CancellationToken cancellationToken);

    Task<EntitlementPurchase?> UpdatePaymentStatusAsync(
        string purchaseId,
        PaymentStatus paymentStatus,
        DateTimeOffset activatesAt,
        CancellationToken cancellationToken);

    Task ConfirmStripeSubscriptionInvoiceAsync(
        string purchaseId,
        string stripeAccountId,
        string stripeInvoiceId,
        DateTimeOffset serviceStartAt,
        string? stripePaymentIntentId,
        CancellationToken cancellationToken);

    Task UpdateStripePaymentContextAsync(
        string purchaseId,
        string? stripeCheckoutSessionId,
        string? stripePaymentIntentId,
        CancellationToken cancellationToken);

    Task<int> ExpirePendingAsync(CancellationToken cancellationToken);

    Task<EntitlementModel?> CompleteAsync(
        string purchaseReference,
        string customerId,
        string organizationId,
        ProductPricing pricing,
        PaymentStatus paymentStatus,
        DateTimeOffset activatesAt,
        string currency,
        CancellationToken cancellationToken);
}

public sealed record EntitlementPurchaseProductModel(ProductPricing Pricing, string Currency, string OrganizationId);

public sealed class EntitlementPurchaseService(
    IEntitlementService entitlementService,
    IRepositoryFactory repositoryFactory,
    IEntitlementPurchaseModelMapper purchaseModelMapper,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IEntitlementPurchasePaymentCancellationService paymentCancellationService,
    IEntitlementInvoiceService entitlementInvoiceService,
    IProductVersionHelperService productVersionHelperService,
    IStripeProductPricingService stripeProductPricingService,
    IRecurringInvoiceBillingScheduleService recurringInvoiceBillingScheduleService,
    IMarketplaceStripeSubscriptionPriceService marketplaceStripeSubscriptionPriceService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    ILogger<EntitlementPurchaseService> logger) : IEntitlementPurchaseService
{
    public Task<EntitlementPurchase> CreatePendingAsync(
        string customerId,
        string organizationId,
        string productVersionId,
        ProductPricing pricing,
        string currency,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentExpiry,
        string? checkoutReturnUrl,
        IReadOnlyCollection<string> invoiceEmailList,
        CancellationToken cancellationToken) =>
        CreatePendingAsync(
            customerId,
            organizationId,
            productVersionId,
            pricing,
            currency,
            paymentMethod,
            paymentExpiry,
            timeProvider.GetUtcNow(),
            checkoutReturnUrl,
            invoiceEmailList,
            false,
            cancellationToken);

    public async Task<EntitlementPurchase?> GetByIdAsync(string purchaseId, CancellationToken cancellationToken) =>
        await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken) is { } purchase
            ? purchaseModelMapper.Map(purchase)
            : null;

    public Task<EntitlementPurchase> CreatePendingAsync(
        string customerId, string organizationId, string productVersionId, ProductPricing pricing, string currency,
        PaymentMethod paymentMethod, DateTimeOffset paymentExpiry, DateTimeOffset serviceStartAt, string? checkoutReturnUrl,
        IReadOnlyCollection<string> invoiceEmailList, CancellationToken cancellationToken) =>
        CreatePendingAsync(customerId, organizationId, productVersionId, pricing, currency, paymentMethod, paymentExpiry,
            serviceStartAt, checkoutReturnUrl, invoiceEmailList, false, cancellationToken);

    public async Task SetCheckoutReturnUrlAsync(string purchaseId, string checkoutReturnUrl, CancellationToken cancellationToken)
    {
        var updated = await repositoryFactory.EntitlementPurchaseRepository.UpdateCheckoutReturnUrlAsync(
            purchaseId,
            checkoutReturnUrl,
            cancellationToken);
        if (!updated)
        {
            throw new InvalidOperationException("The entitlement purchase could not be updated with the checkout return URL.");
        }
    }

    public async Task<IReadOnlyList<EntitlementPurchase>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken) =>
    [
        .. (await repositoryFactory.EntitlementPurchaseRepository.GetForCustomerAsync(customerId, cancellationToken)).Select(purchaseModelMapper.Map),
    ];

    public async Task<IReadOnlyList<EntitlementPurchase>> GetForOrganizationAsync(string organizationId, CancellationToken cancellationToken) =>
    [
        .. (await repositoryFactory.EntitlementPurchaseRepository.GetForOrganizationAsync(organizationId, cancellationToken))
        .Select(purchaseModelMapper.Map),
    ];

    public async Task<EntitlementPurchaseProductModel?> GetProductAsync(string productVersionId, string pricingId,
        CancellationToken cancellationToken)
    {
        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(productVersionId, cancellationToken);
        var pricing = productVersion?.PricingOptions?.SingleOrDefault(item => item.Id == pricingId);
        return productVersion is null || pricing is null
            ? null
            : new EntitlementPurchaseProductModel(pricing, productVersion.Currency ?? string.Empty, productVersion.Product.OrganizationId);
    }

    public async Task<EntitlementPurchase> CreatePendingAsync(
        string customerId,
        string organizationId,
        string productVersionId,
        ProductPricing pricing,
        string currency,
        PaymentMethod paymentMethod,
        DateTimeOffset paymentExpiry,
        DateTimeOffset serviceStartAt,
        string? checkoutReturnUrl,
        IReadOnlyCollection<string> invoiceEmailList,
        bool autoRenew,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(productVersionId);

        if (pricing.FulfillmentType != ProductPricingFulfillmentType.Entitlement)
        {
            throw new InvalidOperationException("Only entitlement pricing can create a token purchase.");
        }

        if (pricing.EntitlementCreditQuantity is not > 0 || pricing.EntitlementValidityDays is not > 0)
        {
            throw new EntitlementPricingConfigurationInvalid();
        }

        if (!pricing.AcceptedPaymentMethods.Contains(paymentMethod))
        {
            throw new InvalidOperationException("The selected payment method is not accepted for this entitlement purchase.");
        }

        if (autoRenew && (!pricing.SupportsSubscriptionAutoRenewal || paymentMethod != PaymentMethod.Card))
        {
            throw new InvalidOperationException(
                "Auto-renewing entitlement purchases require card payment and a pricing option that supports AutoRenew.");
        }

        var purchase = new Database.Entities.EntitlementPurchase
        {
            Id = randomHelper.Generate(),
            CreatedAt = timeProvider.GetUtcNow(),
            AutoRenew = autoRenew,
            PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
            PaymentMethod = paymentMethod.ToPaymentMethod(),
            PaymentExpiry = paymentExpiry,
            ServiceStartAt = serviceStartAt,
            Amount = pricing.Price,
            Currency = currency,
            ProductPricing = pricing,
            CheckoutReturnUrl = checkoutReturnUrl,
            InvoiceEmailList = [.. invoiceEmailList],
            CustomerId = customerId,
            OrganizationId = organizationId,
            ProductVersionId = productVersionId,
        };

        repositoryFactory.EntitlementPurchaseRepository.Add(purchase);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(purchase.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await entitlementInvoiceService.GenerateAsync(purchase.Id, cancellationToken);
        logger.LogInformation(
            "Created pending entitlement purchase. PurchaseId={PurchaseId}, CustomerId={CustomerId}, OrganizationId={OrganizationId}, ProductVersionId={ProductVersionId}, PricingId={PricingId}, PaymentMethod={PaymentMethod}, Amount={Amount}, Currency={Currency}, PaymentExpiry={PaymentExpiry}",
            purchase.Id, customerId, organizationId, productVersionId, pricing.Id, purchase.PaymentMethod, purchase.Amount, purchase.Currency,
            purchase.PaymentExpiry);
        return purchaseModelMapper.Map(purchase);
    }

    public async Task<EntitlementModel> ConfirmAsync(
        string purchaseId,
        DateTimeOffset activatesAt,
        CancellationToken cancellationToken)
    {
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken) ??
                       throw new InvalidOperationException("The entitlement purchase could not be found.");

        if (purchase.EntitlementId is not null)
        {
            logger.LogInformation("Entitlement purchase confirmation is idempotent. PurchaseId={PurchaseId}, EntitlementId={EntitlementId}",
                purchase.Id, purchase.EntitlementId);
            var existingEntitlement = await entitlementService.GrantAsync(
                                          purchase.Id,
                                          purchase.CustomerId,
                                          purchase.OrganizationId,
                                          await ResolvePaidInstallmentPricingAsync(purchase, purchase.ServiceStartAt, cancellationToken),
                                          purchase.ServiceStartAt,
                                          purchase.Currency,
                                          false,
                                          cancellationToken) ??
                                      throw new InvalidOperationException("The entitlement purchase could not be completed.");
            await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(
                purchase.Id,
                cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(GraphQlConstants.EntitlementPurchaseTopicName, purchase.Id, cancellationToken);
            return existingEntitlement;
        }

        purchase.PaymentStatus = PaymentStatus.Confirmed.ToPaymentStatus();
        purchase.PaymentConfirmedAt = timeProvider.GetUtcNow();
        var entitlement = await entitlementService.GrantAsync(
                              purchase.Id,
                              purchase.CustomerId,
                              purchase.OrganizationId,
                              await ResolvePaidInstallmentPricingAsync(purchase, purchase.ServiceStartAt, cancellationToken),
                              purchase.ServiceStartAt,
                              purchase.Currency,
                              false,
                              cancellationToken) ??
                          throw new InvalidOperationException("The entitlement purchase could not be completed.");

        purchase.EntitlementId = entitlement.Id;
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(purchase.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(GraphQlConstants.EntitlementPurchaseTopicName, purchase.Id, cancellationToken);
        logger.LogInformation(
            "Confirmed entitlement purchase and granted cycle. PurchaseId={PurchaseId}, EntitlementId={EntitlementId}, CustomerId={CustomerId}, OrganizationId={OrganizationId}",
            purchase.Id, entitlement.Id, purchase.CustomerId, purchase.OrganizationId);
        return entitlement;
    }

    public async Task<EntitlementPurchase?> UpdatePaymentStatusAsync(
        string purchaseId,
        PaymentStatus paymentStatus,
        DateTimeOffset activatesAt,
        CancellationToken cancellationToken)
    {
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken);
        if (purchase is null)
        {
            return null;
        }

        // A payment provider callback can arrive after the local payment deadline. The
        // expiry workflow is authoritative for pending purchases, so never let a late
        // confirmation grant a cycle after that boundary.
        if (paymentStatus == PaymentStatus.Confirmed &&
            purchase.EntitlementId is null &&
            purchase.PaymentStatus == PaymentStatusConstants.Pending &&
            purchase.PaymentExpiry <= timeProvider.GetUtcNow())
        {
            purchase.PaymentStatus = PaymentStatusConstants.Expired;
            purchase.FailureReason = "The entitlement purchase payment deadline has passed.";
            await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(purchase.Id, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(GraphQlConstants.EntitlementPurchaseTopicName, purchase.Id, cancellationToken);
            logger.LogWarning(
                "Rejected late entitlement purchase confirmation. PurchaseId={PurchaseId}, PaymentExpiry={PaymentExpiry}",
                purchase.Id,
                purchase.PaymentExpiry);
            return purchaseModelMapper.Map(purchase);
        }

        if (paymentStatus == PaymentStatus.Confirmed)
        {
            if (!purchase.AutoRenew && purchase.PaymentMethod == PaymentMethod.Card.ToPaymentMethod() &&
                (string.IsNullOrWhiteSpace(purchase.StripeCheckoutSessionId) ||
                 string.IsNullOrWhiteSpace(purchase.StripePaymentIntentId)))
            {
                logger.LogWarning(
                    "Rejected entitlement purchase confirmation without Stripe payment context. PurchaseId={PurchaseId}, PaymentStatus={PaymentStatus}",
                    purchase.Id,
                    purchase.PaymentStatus);
                return purchaseModelMapper.Map(purchase);
            }

            if (purchase.EntitlementId is null && purchase.PaymentStatus != PaymentStatus.Pending.ToPaymentStatus())
            {
                logger.LogWarning(
                    "Ignored entitlement purchase confirmation after a terminal payment state. PurchaseId={PurchaseId}, PaymentStatus={PaymentStatus}",
                    purchase.Id,
                    purchase.PaymentStatus);
                return purchaseModelMapper.Map(purchase);
            }

            await ConfirmAsync(purchaseId, activatesAt, cancellationToken);
            await entitlementInvoiceService.GenerateAsync(purchaseId, cancellationToken);
            purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken) ?? purchase;
        }
        else if (purchase.EntitlementId is null && purchase.PaymentStatus == PaymentStatus.Pending.ToPaymentStatus())
        {
            if (paymentStatus == PaymentStatus.NoPaymentRequired)
            {
                await ConfirmAsync(purchaseId, activatesAt, cancellationToken);
                await entitlementInvoiceService.GenerateAsync(purchaseId, cancellationToken);
                purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken) ?? purchase;
                purchase.PaymentStatus = PaymentStatus.NoPaymentRequired.ToPaymentStatus();
            }
            else
            {
                purchase.PaymentStatus = paymentStatus.ToPaymentStatus();
                purchase.PaymentConfirmedAt = null;
            }

            await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(purchase.Id, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(GraphQlConstants.EntitlementPurchaseTopicName, purchase.Id, cancellationToken);
            logger.LogInformation(
                "Updated entitlement purchase payment status. PurchaseId={PurchaseId}, CustomerId={CustomerId}, OrganizationId={OrganizationId}, PaymentStatus={PaymentStatus}",
                purchase.Id,
                purchase.CustomerId,
                purchase.OrganizationId,
                purchase.PaymentStatus);
        }

        return purchaseModelMapper.Map(purchase);
    }

    public async Task UpdateStripePaymentContextAsync(
        string purchaseId,
        string? stripeCheckoutSessionId,
        string? stripePaymentIntentId,
        CancellationToken cancellationToken)
    {
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken);
        if (purchase is null)
        {
            return;
        }

        purchase.StripeCheckoutSessionId ??= stripeCheckoutSessionId;
        purchase.StripePaymentIntentId ??= stripePaymentIntentId;
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(purchase.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ConfirmStripeSubscriptionInvoiceAsync(
        string purchaseId,
        string stripeAccountId,
        string stripeInvoiceId,
        DateTimeOffset serviceStartAt,
        string? stripePaymentIntentId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stripeAccountId);
        ArgumentException.ThrowIfNullOrWhiteSpace(stripeInvoiceId);

        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken) ??
                       throw new InvalidOperationException("The entitlement purchase could not be found.");
        if (!purchase.AutoRenew || purchase.PaymentMethod != PaymentMethod.Card.ToPaymentMethod())
        {
            logger.LogWarning(
                "Ignored Stripe subscription invoice for a purchase without Stripe Billing authorization. PurchaseId={PurchaseId}, InvoiceId={InvoiceId}",
                purchaseId,
                stripeInvoiceId);
            return;
        }

        if (purchase.EntitlementId is null)
        {
            // Checkout completion captures authorization only. The first paid invoice is
            // the sole confirmation gate for the initial credit entitlement.
            purchase.StripeInvoiceId = stripeInvoiceId;
            purchase.StripePaymentIntentId ??= stripePaymentIntentId;
            repositoryFactory.EntitlementPurchaseRepository.Update(purchase);
            await ConfirmAsync(purchase.Id, serviceStartAt, cancellationToken);
            await entitlementInvoiceService.GenerateAsync(purchase.Id, cancellationToken);
            return;
        }

        var invoicePurchaseReference = $"stripe-subscription-invoice:{stripeAccountId}:{stripeInvoiceId}";
        var renewalPurchase = await repositoryFactory.EntitlementPurchaseRepository
            .GetByIdAsync(invoicePurchaseReference, cancellationToken);
        var installmentPricing = renewalPurchase?.ProductPricing ??
                                 await ResolvePaidInstallmentPricingAsync(purchase, serviceStartAt, cancellationToken);
        if (renewalPurchase is null)
        {
            var validityDays = installmentPricing.EntitlementValidityDays ?? 0;
            renewalPurchase = new Database.Entities.EntitlementPurchase
            {
                Id = invoicePurchaseReference,
                CreatedAt = timeProvider.GetUtcNow(),
                AutoRenew = false,
                PaymentStatus = PaymentStatus.Confirmed.ToPaymentStatus(),
                PaymentMethod = purchase.PaymentMethod,
                PaymentConfirmedAt = timeProvider.GetUtcNow(),
                StripeInvoiceId = stripeInvoiceId,
                StripePaymentIntentId = stripePaymentIntentId,
                StripeCustomerId = purchase.StripeCustomerId,
                PaymentExpiry = validityDays > 0 ? serviceStartAt.AddDays(validityDays) : serviceStartAt,
                ServiceStartAt = serviceStartAt,
                Amount = installmentPricing.Price,
                Currency = purchase.Currency,
                ProductPricing = installmentPricing,
                CustomerId = purchase.CustomerId,
                OrganizationId = purchase.OrganizationId,
                ProductVersionId = purchase.ProductVersionId,
            };
        }

        var entitlement = await entitlementService.GrantAsync(
            invoicePurchaseReference,
            purchase.CustomerId,
            purchase.OrganizationId,
            installmentPricing,
            serviceStartAt,
            purchase.Currency,
            cancellationToken);
        if (renewalPurchase.EntitlementId is null)
        {
            renewalPurchase.EntitlementId = entitlement.Id;
            if (renewalPurchase.Id == invoicePurchaseReference &&
                await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(invoicePurchaseReference, cancellationToken) is null)
            {
                repositoryFactory.EntitlementPurchaseRepository.Add(renewalPurchase);
            }
            else
            {
                repositoryFactory.EntitlementPurchaseRepository.Update(renewalPurchase);
            }

            try
            {
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                // Two deliveries can race after both miss the invoice reference.
                // The invoice reference is the idempotency boundary; reload the
                // winner and continue processing the already-created entitlement.
                repositoryFactory.ResetChangeTracker();
                renewalPurchase = await repositoryFactory.EntitlementPurchaseRepository
                    .GetByIdAsync(invoicePurchaseReference, cancellationToken);
                if (renewalPurchase is null)
                {
                    throw;
                }
            }
        }

        await repositoryFactory.MarketplacePurchaseHistoryRepository.AppendEventAsync(
            new MarketplacePurchaseHistoryEventModel(
                $"stripe-invoice-paid:{stripeAccountId}:{stripeInvoiceId}",
                purchase.Id,
                MarketplacePurchaseHistoryEligibleSourceType.Entitlement,
                MarketplacePurchaseHistoryEventType.EntitlementCreated,
                serviceStartAt,
                timeProvider.GetUtcNow(),
                null,
                PaymentStatus.Confirmed,
                null,
                null,
                null,
                entitlement.GrantedQuantity,
                entitlement.GrantedQuantity,
                installmentPricing.Price,
                null,
                null,
                null,
                null,
                "Stripe subscription invoice paid",
                null,
                entitlement.Status,
                true,
                null,
                null,
                $"stripe-account:{stripeAccountId};stripe-invoice:{stripeInvoiceId}"),
            $"stripe-invoice-paid:{stripeAccountId}:{stripeInvoiceId}",
            cancellationToken);

        // The paid invoice is authoritative for this grant. Only after the grant is
        // recorded may the next Stripe Billing cycle be moved to the current price.
        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(purchase.ProductVersionId, cancellationToken);
        var currentPricing = productVersion is null
            ? null
            : productVersionHelperService.FindMatchingPricing([.. productVersion.PricingOptions ?? []], purchase.ProductPricing);
        var currentStripePrice = currentPricing is null || productVersion is null || string.IsNullOrWhiteSpace(purchase.StripeAccountId)
            ? null
            : await EnsureEffectiveRecurringPriceAsync(purchase, productVersion, currentPricing, cancellationToken);
        if (currentPricing is not null && !string.IsNullOrWhiteSpace(currentStripePrice) &&
            !string.Equals(currentPricing.Id, purchase.ProductPricing.Id, StringComparison.Ordinal))
        {
            if (await marketplaceStripeSubscriptionPriceService.UpdatePriceForEntitlementAsync(
                    purchase.Id,
                    currentStripePrice,
                    1,
                    cancellationToken))
            {
                purchase.ProductPricing = currentPricing;
                purchase.Amount = currentPricing.Price;
                repositoryFactory.EntitlementPurchaseRepository.Update(purchase);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        logger.LogInformation(
            "Granted entitlement from paid Stripe subscription invoice. PurchaseId={PurchaseId}, InvoiceId={InvoiceId}, EntitlementId={EntitlementId}",
            purchase.Id,
            stripeInvoiceId,
            entitlement.Id);
    }

    public async Task<EntitlementModel?> CompleteAsync(
        string purchaseReference,
        string customerId,
        string organizationId,
        ProductPricing pricing,
        PaymentStatus paymentStatus,
        DateTimeOffset activatesAt,
        string currency,
        CancellationToken cancellationToken)
    {
        if (pricing.FulfillmentType != ProductPricingFulfillmentType.Entitlement ||
            paymentStatus is not (PaymentStatus.Confirmed or PaymentStatus.NoPaymentRequired))
        {
            logger.LogDebug(
                "Skipped entitlement grant because payment is not confirmed or pricing is not token based. PurchaseReference={PurchaseReference}, PricingId={PricingId}, FulfillmentType={FulfillmentType}, PaymentStatus={PaymentStatus}",
                purchaseReference,
                pricing.Id,
                pricing.FulfillmentType,
                paymentStatus);
            return null;
        }

        logger.LogInformation(
            "Starting entitlement grant after confirmed payment. PurchaseReference={PurchaseReference}, CustomerId={CustomerId}, OrganizationId={OrganizationId}, PricingId={PricingId}, PaymentStatus={PaymentStatus}, Amount={Amount}, Currency={Currency}",
            purchaseReference,
            customerId,
            organizationId,
            pricing.Id,
            paymentStatus,
            pricing.Price,
            currency);
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseReference, cancellationToken);
        var entitlement = await entitlementService.GrantAsync(
            purchaseReference,
            customerId,
            organizationId,
            pricing,
            purchase?.ServiceStartAt ?? activatesAt,
            currency,
            cancellationToken);

        // CompleteAsync is used by payment-provider callbacks. Keep the purchase
        // linked to the granted entitlement so entitlement reads can recover the
        // product restrictions and purchase details.
        if (purchase is not null && purchase.EntitlementId is null)
        {
            purchase.EntitlementId = entitlement.Id;
            await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(
                purchase.Id,
                cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }

        return entitlement;
    }

    public async Task<int> ExpirePendingAsync(CancellationToken cancellationToken)
    {
        var purchases = await repositoryFactory.EntitlementPurchaseRepository
            .GetExpiredPendingAsync(timeProvider.GetUtcNow(), cancellationToken);
        foreach (var purchase in purchases)
        {
            try
            {
                await paymentCancellationService.CancelAsync(purchase, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogWarning(exception,
                    "Unable to stop entitlement purchase payment workflow; local expiry remains authoritative. PurchaseId={PurchaseId}", purchase.Id);
            }

            purchase.PaymentStatus = PaymentStatus.Expired.ToPaymentStatus();
            purchase.FailureReason = "Payment was not confirmed before the entitlement purchase deadline.";
        }

        if (purchases.Count > 0)
        {
            foreach (var purchase in purchases)
            {
                await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(purchase.Id, cancellationToken);
            }

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Expired pending entitlement purchases. PurchaseCount={PurchaseCount}, ExpiredAt={ExpiredAt}", purchases.Count,
                timeProvider.GetUtcNow());
        }

        return purchases.Count;
    }

    private async Task<ProductPricing> ResolvePaidInstallmentPricingAsync(
        Database.Entities.EntitlementPurchase purchase,
        DateTimeOffset serviceStartAt,
        CancellationToken cancellationToken)
    {
        var pricing = purchase.ProductPricing;
        if (!purchase.AutoRenew || pricing.BillingMode != ProductPricingBillingMode.InArrears)
        {
            return pricing;
        }

        var productVersion = await repositoryFactory.ProductVersionRepository
            .GetByIdAsync(purchase.ProductVersionId, cancellationToken);
        var organizationBillingCycle = productVersion?.Product?.Organization?.BillingCycle;
        if (productVersion is null || organizationBillingCycle is null)
        {
            return pricing;
        }

        var schedule = recurringInvoiceBillingScheduleService.GetSchedule(
            pricing,
            purchase.Amount,
            organizationBillingCycle.ToOrganizationBillingCycle());
        if (schedule.MembershipTerm == pricing.MembershipTerm)
        {
            return pricing;
        }

        var installmentCount = schedule.InstallmentCount;
        var totalCredits = pricing.EntitlementCreditQuantity ?? 0;
        var totalValidityDays = pricing.EntitlementValidityDays ?? 0;
        if (totalCredits < installmentCount || totalValidityDays < installmentCount)
        {
            throw new InvalidOperationException(
                "The entitlement credit quantity and validity period must each cover every billing installment.");
        }

        var installmentIndex = MarketplaceEntitlementInstallmentAllocation.GetIndex(
            purchase.ServiceStartAt,
            serviceStartAt,
            organizationBillingCycle.ToOrganizationBillingCycle(),
            installmentCount);
        var installmentCredits = MarketplaceEntitlementInstallmentAllocation.GetAmount(
            totalCredits, installmentCount, installmentIndex);
        var installmentValidityDays = MarketplaceEntitlementInstallmentAllocation.GetAmount(
            totalValidityDays, installmentCount, installmentIndex);

        return pricing with
        {
            Price = schedule.InvoiceAmount,
            EntitlementCreditQuantity = installmentCredits,
            EntitlementValidityDays = installmentValidityDays,
        };
    }

    private async Task<string?> EnsureEffectiveRecurringPriceAsync(
        Database.Entities.EntitlementPurchase purchase,
        ProductVersion productVersion,
        ProductPricing pricing,
        CancellationToken cancellationToken)
    {
        var schedule = recurringInvoiceBillingScheduleService.GetSchedule(
            pricing,
            pricing.Price,
            productVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());
        return await stripeProductPricingService.EnsureRecurringPriceAsync(
            productVersion,
            pricing,
            purchase.StripeAccountId!,
            schedule.InvoiceAmount,
            schedule.MembershipTerm,
            cancellationToken);
    }

    private async Task<EntitlementModel?> GrantAsync(string purchaseReference, string customerId, string organizationId, ProductPricing pricing,
        DateTimeOffset activatesAt, string currency, CancellationToken cancellationToken) =>
        await entitlementService.GrantAsync(purchaseReference, customerId, organizationId, pricing, activatesAt, currency, cancellationToken);
}
