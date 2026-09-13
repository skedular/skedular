using System.Globalization;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Stripe;
using Stripe.Checkout;
using MarketplaceStripeConfiguration = Enterprise.Shared.Payment.Configurations.StripeConfiguration;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using PaymentMethod = Api.Shared.Services.Models.PaymentMethod;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementPurchaseCheckoutService
{
    Task<EntitlementPurchaseCheckoutAction> CreateCardCheckoutAsync(string purchaseId, CancellationToken cancellationToken);
}

public sealed record EntitlementPurchaseCheckoutAction(string CheckoutUrl);

/// <summary>
///     Creates the customer-visible payment action for a standalone entitlement purchase.
///     This is intentionally a service rather than a Temporal activity: the create-purchase
///     mutation must return the Checkout URL synchronously to satisfy the purchase contract.
/// </summary>
public sealed class EntitlementPurchaseCheckoutService(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    OrganizationConfiguration organizationConfiguration,
    OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
    IStripeProductPricingService stripeProductPricingService,
    IStripeCustomerService stripeCustomerService,
    MarketplaceStripeConfiguration stripeConfiguration,
    ICreatable<Session, SessionCreateOptions> sessionCreateService) : IEntitlementPurchaseCheckoutService
{
    public async Task<EntitlementPurchaseCheckoutAction> CreateCardCheckoutAsync(string purchaseId, CancellationToken cancellationToken)
    {
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseId, cancellationToken)
                       ?? throw new InvalidOperationException("The entitlement purchase could not be found.");
        if (purchase.PaymentStatus != PaymentStatusConstants.Pending ||
            purchase.PaymentMethod != PaymentMethod.Card.ToPaymentMethod())
        {
            throw new InvalidOperationException("Only pending card entitlement purchases can create a checkout session.");
        }

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(purchase.ProductVersionId, cancellationToken)
                             ?? throw new ProductVersionNotFound();
        ArgumentNullException.ThrowIfNull(productVersion.Product?.Organization);

        var accounts = await organizationBillingServiceClient.Admin_GetStripeConnectAccountsAsync(
            new Admin_GetStripeConnectAccountsInput
            {
                After = string.Empty,
                First = ((int?)null).ToNullInt(),
                Before = string.Empty,
                Last = ((int?)null).ToNullInt(),
                Where = new StripeConnectAccountWhereInput
                {
                    OrganizationId = productVersion.Product.Organization.Id,
                    OnboardingCompleted = true,
                },
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        var stripeAccountId = accounts.Edges.Select(item => item.Node).First(item => item.IsDefault).StripeAccountId;

        await stripeProductPricingService.EnsureProductPricingAsync(
            productVersion, purchase.ProductPricing, stripeAccountId, cancellationToken);
        var stripeProduct = productVersion.StripeProducts.FirstOrDefault(item => item.StripeAccountId == stripeAccountId)
                            ?? throw new InvalidOperationException($"Stripe product is not configured for pricing {purchase.ProductPricing.Id}.");
        var oneTimeStripePriceId = stripeProductPricingService.GetOneTimePriceId(
            productVersion, purchase.ProductPricing, stripeAccountId);
        var usesStripeSubscription = stripeConfiguration.EnableMarketplaceBillingAutoRenewal && purchase.AutoRenew;
        var recurringStripePriceId = usesStripeSubscription
            ? stripeProductPricingService.GetRecurringPriceId(productVersion, purchase.ProductPricing, stripeAccountId)
            : null;
        if (usesStripeSubscription && string.IsNullOrWhiteSpace(recurringStripePriceId))
        {
            throw new InvalidOperationException("The auto-renewing entitlement price has no recurring Stripe Price.");
        }

        if (!usesStripeSubscription)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(oneTimeStripePriceId);
        }

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(purchase.CustomerId, true, cancellationToken)
                       ?? throw new CustomerNotFound();
        var stripeCustomer = await stripeCustomerService.AddCustomerAsync(customer, stripeAccountId, cancellationToken);
        var returnUrl = purchase.CheckoutReturnUrl ?? applicationConfiguration.WebAppBaseDomain.ToString();
        var session = await sessionCreateService.CreateAsync(
            new SessionCreateOptions
            {
                Customer = stripeCustomer.StripeCustomerId,
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        Price = usesStripeSubscription ? recurringStripePriceId : oneTimeStripePriceId,
                        Quantity = 1,
                    },
                ],
                Mode = usesStripeSubscription ? "subscription" : "payment",
                UiMode = "hosted_page",
                PaymentMethodTypes = ["card"],
                ClientReferenceId = purchase.Id,
                Metadata = new Dictionary<string, string>
                {
                    ["purchase_id"] = purchase.Id,
                    ["marketplace_purchase_type"] = "entitlement",
                    ["marketplace_purchase_id"] = purchase.Id,
                    ["pricing_id"] = purchase.ProductPricing.Id,
                    ["amount"] = purchase.Amount.ToString(CultureInfo.InvariantCulture),
                    ["currency"] = purchase.Currency,
                    ["marketplace_stripe_price_id"] = usesStripeSubscription ? recurringStripePriceId! : oneTimeStripePriceId!,
                },
                SubscriptionData = usesStripeSubscription
                    ? new SessionSubscriptionDataOptions
                    {
                        Metadata = new Dictionary<string, string>
                        {
                            ["marketplace_purchase_type"] = "entitlement",
                            ["marketplace_purchase_id"] = purchase.Id,
                            ["marketplace_stripe_price_id"] = recurringStripePriceId!,
                        },
                    }
                    : null,
                SuccessUrl = returnUrl,
                CancelUrl = returnUrl,
                AutomaticTax = new SessionAutomaticTaxOptions
                {
                    Enabled = true,
                },
            },
            new RequestOptions
            {
                IdempotencyKey = purchase.Id,
                StripeAccount = stripeAccountId,
            },
            cancellationToken);

        var checkoutUrl = session.Url ?? throw new InvalidOperationException("Stripe did not return a checkout URL.");
        var updated = await repositoryFactory.EntitlementPurchaseRepository.UpdateCardCheckoutAsync(
            purchase.Id,
            session.Id,
            checkoutUrl,
            session.PaymentIntentId,
            stripeAccountId,
            cancellationToken);
        if (!updated)
        {
            throw new InvalidOperationException("The entitlement purchase could not be updated with the Stripe checkout session.");
        }

        return new EntitlementPurchaseCheckoutAction(checkoutUrl);
    }
}
