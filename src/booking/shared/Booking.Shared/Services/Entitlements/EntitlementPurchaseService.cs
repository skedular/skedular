using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;
using GraphQlConstants = Booking.Shared.GraphQL.Constants;
using EntitlementPurchase = Booking.Shared.Models.Entitlements.EntitlementPurchase;

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
            pricing.SupportsSubscriptionAutoRenewal,
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
            serviceStartAt, checkoutReturnUrl, invoiceEmailList, pricing.SupportsSubscriptionAutoRenewal, cancellationToken);

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

        var purchase = new Database.Entities.EntitlementPurchase
        {
            Id = randomHelper.Generate(),
            CreatedAt = timeProvider.GetUtcNow(),
            PaymentStatus = PaymentStatus.Pending.ToPaymentStatus(),
            PaymentMethod = paymentMethod.ToPaymentMethod(),
            AutoRenew = autoRenew && pricing.SupportsSubscriptionAutoRenewal,
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
                                          purchase.ProductPricing,
                                          purchase.ServiceStartAt,
                                          purchase.Currency,
                                          purchase.AutoRenew,
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
                              purchase.ProductPricing,
                              purchase.ServiceStartAt,
                              purchase.Currency,
                              purchase.AutoRenew,
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
            if (purchase.PaymentMethod == PaymentMethod.Card.ToPaymentMethod() &&
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

            if (!string.IsNullOrWhiteSpace(purchase.RenewalOfPurchaseId))
            {
                var sourcePurchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(
                    purchase.RenewalOfPurchaseId,
                    cancellationToken);
                if (sourcePurchase?.EntitlementId is { } entitlementId)
                {
                    var entitlement = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken);
                    if (entitlement is not null)
                    {
                        entitlement.RenewalFailureReason = purchase.FailureReason;
                        entitlement.NextRenewalAt = null;
                    }
                }
            }
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

    private async Task<EntitlementModel?> GrantAsync(string purchaseReference, string customerId, string organizationId, ProductPricing pricing,
        DateTimeOffset activatesAt, string currency, CancellationToken cancellationToken) =>
        await entitlementService.GrantAsync(purchaseReference, customerId, organizationId, pricing, activatesAt, currency, cancellationToken);
}
