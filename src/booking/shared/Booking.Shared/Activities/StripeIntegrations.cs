using System.Globalization;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using Temporalio.Activities;
using Constants = Booking.Shared.GraphQL.Constants;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using StripeCustomer = Booking.Shared.Database.Entities.StripeCustomer;

namespace Booking.Shared.Activities;

public record CreateCheckoutSessionAsyncInput(string BookingId, string StripeConnectAccountId, string StripeCustomerId);

public record CreateRecurringBookingCheckoutSessionAsyncInput(string RecurringBookingId, string StripeConnectAccountId, string StripeCustomerId);

public record UpsertBookingRelatedStripeCustomerInput(string BookingId, string StripeConnectAccountId);

public record UpsertRecurringBookingRelatedStripeCustomerInput(string RecurringBookingId, string StripeConnectAccountId);

public record UpsertProductAndPricingInput(string BookingId);

public record UpsertRecurringBookingProductAndPricingInput(string RecurringBookingId);

public record CreateCheckoutSessionAsyncResponse(string PaymentStatus);

public record CreateEntitlementCheckoutSessionAsyncInput(string PurchaseId, string StripeConnectAccountId, string StripeCustomerId);

public record CreateEntitlementCheckoutSessionAsyncResponse(string CheckoutUrl, string PaymentStatus);

public record UpsertBookingRelatedStripeCustomerResponse(string StripeCustomerId);

public record UpsertProductAndPricingResponse(string StripeConnectAccountId);

public class StripeIntegrations(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    OrganizationConfiguration organizationConfiguration,
    OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
    IStripeProductPricingService stripeProductPricingService,
    IStripeCustomerService stripeCustomerService,
    ICreatable<Session, SessionCreateOptions> sessionCreateService,
    IRandomHelper randomHelper,
    IEntityMapper entityMapper,
    IOrganizationArrearsBillingPlannerService organizationArrearsBillingPlannerService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IHostStripeApplicationFeeService hostStripeApplicationFeeService,
    ILogger<StripeIntegrations> logger)
{
    [Activity]
    public async Task<UpsertProductAndPricingResponse?> UpsertProductAndPricingAsync(UpsertProductAndPricingInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted())
        {
            return null;
        }

        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();

        var stripeConnectAccountConnection = await organizationBillingServiceClient.Admin_GetStripeConnectAccountsAsync(
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
        var stripeConnectAccountId = stripeConnectAccountConnection.Edges.Select(item => item.Node).First(item => item.IsDefault).StripeAccountId;

        await stripeProductPricingService.UpsertProductPricingAsync(productVersion, stripeConnectAccountId, cancellationToken);

        return new UpsertProductAndPricingResponse(stripeConnectAccountId);
    }

    [Activity]
    public async Task<UpsertProductAndPricingResponse?> UpsertRecurringBookingProductAndPricingAsync(
        UpsertRecurringBookingProductAndPricingInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted() || recurringBooking.MarketplaceBooking is null)
        {
            return null;
        }

        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();

        var stripeConnectAccountConnection = await organizationBillingServiceClient.Admin_GetStripeConnectAccountsAsync(
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
        var stripeConnectAccountId = stripeConnectAccountConnection.Edges.Select(item => item.Node).First(item => item.IsDefault).StripeAccountId;

        await stripeProductPricingService.UpsertProductPricingAsync(productVersion, stripeConnectAccountId, cancellationToken);

        return new UpsertProductAndPricingResponse(stripeConnectAccountId);
    }

    [Activity]
    public async Task<UpsertBookingRelatedStripeCustomerResponse?> UpsertBookingRelatedStripeCustomerAsync(
        UpsertBookingRelatedStripeCustomerInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted() || booking.MarketplaceBooking is null)
        {
            return null;
        }

        StripeCustomer stripeCustomer;
        var marketplaceBooking = booking.MarketplaceBooking;
        if (marketplaceBooking.PaidByCustomer is not null)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(marketplaceBooking.PaidByCustomer.Id, true,
                               cancellationToken) ??
                           throw new CustomerNotFound();
            stripeCustomer = await stripeCustomerService.AddCustomerAsync(customer, args.StripeConnectAccountId, cancellationToken);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (marketplaceBooking.PaidByOrganization is not null)
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                   marketplaceBooking.PaidByOrganization.Id,
                                   null,
                                   false,
                                   false,
                                   cancellationToken) ??
                               throw new OrganizationNotFound();
            stripeCustomer = await stripeCustomerService.AddCustomerAsync(organization, args.StripeConnectAccountId, cancellationToken);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new InvalidOperationException();
        }

        return new UpsertBookingRelatedStripeCustomerResponse(stripeCustomer.StripeCustomerId);
    }

    [Activity]
    public async Task<UpsertBookingRelatedStripeCustomerResponse?> UpsertRecurringBookingRelatedStripeCustomerAsync(
        UpsertRecurringBookingRelatedStripeCustomerInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted() || recurringBooking.MarketplaceBooking is null)
        {
            return null;
        }

        StripeCustomer stripeCustomer;
        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        if (marketplaceBooking.PaidByCustomer is not null)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(marketplaceBooking.PaidByCustomer.Id, true,
                               cancellationToken) ??
                           throw new CustomerNotFound();
            stripeCustomer = await stripeCustomerService.AddCustomerAsync(customer, args.StripeConnectAccountId, cancellationToken);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (marketplaceBooking.PaidByOrganization is not null)
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                   marketplaceBooking.PaidByOrganization.Id,
                                   null,
                                   false,
                                   false,
                                   cancellationToken) ??
                               throw new OrganizationNotFound();
            stripeCustomer = await stripeCustomerService.AddCustomerAsync(organization, args.StripeConnectAccountId, cancellationToken);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            throw new InvalidOperationException();
        }

        return new UpsertBookingRelatedStripeCustomerResponse(stripeCustomer.StripeCustomerId);
    }

    [Activity]
    public async Task<CreateCheckoutSessionAsyncResponse?> CreateCheckoutSessionAsync(CreateCheckoutSessionAsyncInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted())
        {
            return null;
        }

        var marketplaceBooking = booking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();

        var stripeProduct = productVersion.StripeProducts.FirstOrDefault(item => item.ProductPricingId == marketplaceBooking.ProductPricing.Id) ??
                            throw new InvalidOperationException(
                                $"Stripe product is not configured for pricing {marketplaceBooking.ProductPricing.Id}.");
        ArgumentNullException.ThrowIfNull(stripeProduct.StripePrice);

        var lineItems = booking.Schedules.Select(schedule => new SessionLineItemOptions
        {
            Price = stripeProduct.StripePrice.StripePriceId,
            Quantity = marketplaceBooking.Quantity,
        }).ToList();

        if (marketplaceBooking.StripeCheckoutSession is not null)
        {
            return new CreateCheckoutSessionAsyncResponse(marketplaceBooking.PaymentStatus);
        }

        // Hosted Stripe checkout must return the customer to the exact storefront page they
        // started from. Marketplace bookings persist that URL up front because the checkout
        // session is created later inside a workflow, outside the original HTTP request.
        var checkoutReturnUrl = marketplaceBooking.CheckoutReturnUrl ?? applicationConfiguration.WebAppBaseDomain.ToString();

        var sessionOptions = new SessionCreateOptions
        {
            Customer = args.StripeCustomerId,
            LineItems = lineItems,
            Mode = "payment",
            UiMode = "hosted_page",
            PaymentMethodTypes = ["card"],
            ClientReferenceId = booking.Id,
            SuccessUrl = checkoutReturnUrl,
            CancelUrl = checkoutReturnUrl,
            AutomaticTax = new SessionAutomaticTaxOptions
            {
                Enabled = true,
            },
            CustomerUpdate = new SessionCustomerUpdateOptions
            {
                Address = "auto",
                Shipping = "auto",
            },
        };
        var hostPaymentIntentData = hostStripeApplicationFeeService.CreateDestinationCharge(
            marketplaceBooking.ProductVersion.Product.Organization.Type,
            args.StripeConnectAccountId,
            marketplaceBooking.HostCommissionAmount);
        if (hostPaymentIntentData is not null)
        {
            // Host charges belong to the platform so Booking can enforce its
            // cancellation policy. Stripe routes the net proceeds to the Host.
            sessionOptions.Customer = null;
            sessionOptions.CustomerUpdate = null;
            sessionOptions.PaymentIntentData = hostPaymentIntentData;
            UseInlineHostPriceData(sessionOptions.LineItems, marketplaceBooking, productVersion);
        }

        var session = await sessionCreateService.CreateAsync(
            sessionOptions,
            new RequestOptions
            {
                IdempotencyKey = booking.Id,
                StripeAccount = hostPaymentIntentData is null ? args.StripeConnectAccountId : null,
            },
            cancellationToken);

        var stripeCustomer =
            await repositoryFactory.StripeCustomerRepository.GetByStripeCustomerIdAsync(args.StripeCustomerId, cancellationToken) ??
            throw new StripeCustomerNotFound();
        var stripeCheckoutSession = new StripeCheckoutSession
        {
            Id = randomHelper.Generate(),
            StripeCheckoutSessionId = session.Id,
            CheckoutUrl = session.Url,
            PaymentIntentId = session.PaymentIntentId,
            ChargeType = hostPaymentIntentData is null ? "Direct" : "Destination",
            StripeAccountId = hostPaymentIntentData is null ? args.StripeConnectAccountId : null,
            DestinationAccountId = hostPaymentIntentData?.TransferData?.Destination,
            StripeCustomer = stripeCustomer,
        };

        stripeCheckoutSession = repositoryFactory.StripeCheckoutSessionRepository.Add(stripeCheckoutSession);
        marketplaceBooking.StripeCheckoutSession = stripeCheckoutSession;
        marketplaceBooking.PaymentStatus = session.PaymentStatus switch
        {
            "no_payment_required" => PaymentStatusConstants.NoPaymentRequired,
            "unpaid" => PaymentStatusConstants.Pending,
            "paid" => PaymentStatusConstants.Confirmed,
            _ => throw new ArgumentOutOfRangeException(null,
                "Unexpected value encountered. Update enum mapping or caller input to include this case."),
        };

        _ = repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);

        return new CreateCheckoutSessionAsyncResponse(marketplaceBooking.PaymentStatus);
    }

    [Activity]
    public async Task<CreateEntitlementCheckoutSessionAsyncResponse?> CreateEntitlementCheckoutSessionAsync(
        CreateEntitlementCheckoutSessionAsyncInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(args.PurchaseId, cancellationToken);
        if (purchase is null || purchase.PaymentStatus != PaymentStatusConstants.Pending)
        {
            return null;
        }

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(purchase.ProductVersionId, cancellationToken) ??
                             throw new ProductVersionNotFound();
        var stripeProduct = productVersion.StripeProducts.FirstOrDefault(item => item.ProductPricingId == purchase.ProductPricing.Id) ??
                            throw new InvalidOperationException($"Stripe product is not configured for pricing {purchase.ProductPricing.Id}.");
        ArgumentNullException.ThrowIfNull(stripeProduct.StripePrice);

        var checkoutReturnUrl = purchase.CheckoutReturnUrl ?? applicationConfiguration.WebAppBaseDomain.ToString();
        var session = await sessionCreateService.CreateAsync(
            new SessionCreateOptions
            {
                Customer = args.StripeCustomerId,
                LineItems =
                [
                    new SessionLineItemOptions
                    {
                        Price = stripeProduct.StripePrice.StripePriceId,
                        Quantity = 1,
                    },
                ],
                Mode = "payment",
                UiMode = "hosted_page",
                PaymentMethodTypes = ["card"],
                ClientReferenceId = purchase.Id,
                Metadata = new Dictionary<string, string>
                {
                    ["purchase_id"] = purchase.Id,
                    ["pricing_id"] = purchase.ProductPricing.Id,
                    ["amount"] = purchase.Amount.ToString(CultureInfo.InvariantCulture),
                    ["currency"] = purchase.Currency,
                },
                SuccessUrl = checkoutReturnUrl,
                CancelUrl = checkoutReturnUrl,
                AutomaticTax = new SessionAutomaticTaxOptions
                {
                    Enabled = true,
                },
            },
            new RequestOptions
            {
                IdempotencyKey = purchase.Id,
                StripeAccount = args.StripeConnectAccountId,
            },
            cancellationToken);

        logger.LogInformation(
            "Created Stripe entitlement checkout session. PurchaseId={PurchaseId}, PricingId={PricingId}, Amount={Amount}, Currency={Currency}, StripeCheckoutSessionId={StripeCheckoutSessionId}",
            purchase.Id,
            purchase.ProductPricing.Id,
            purchase.Amount,
            purchase.Currency,
            session.Id);

        return new CreateEntitlementCheckoutSessionAsyncResponse(session.Url, session.PaymentStatus switch
        {
            "no_payment_required" => PaymentStatusConstants.NoPaymentRequired,
            "paid" => PaymentStatusConstants.Confirmed,
            _ => PaymentStatusConstants.Pending,
        });
    }

    [Activity]
    public async Task<CreateCheckoutSessionAsyncResponse?> CreateRecurringBookingCheckoutSessionAsync(
        CreateRecurringBookingCheckoutSessionAsyncInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted() || recurringBooking.MarketplaceBooking is null)
        {
            return null;
        }

        var marketplaceBooking = recurringBooking.MarketplaceBooking;

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();

        var stripeProduct = productVersion.StripeProducts.First(item => item.ProductPricingId == marketplaceBooking.ProductPricing.Id);
        ArgumentNullException.ThrowIfNull(stripeProduct.StripePrice);

        if (marketplaceBooking.StripeCheckoutSession is not null)
        {
            return new CreateCheckoutSessionAsyncResponse(marketplaceBooking.PaymentStatus);
        }

        var checkoutReturnUrl = marketplaceBooking.CheckoutReturnUrl ?? applicationConfiguration.WebAppBaseDomain.ToString();
        var isInArrears = marketplaceBooking.BillingMode.ToProductPricingBillingMode() == ProductPricingBillingMode.InArrears;
        List<SessionLineItemOptions> lineItems;

        if (isInArrears)
        {
            var recurringBookingModel = entityMapper.MapTo(recurringBooking);
            var draft = organizationArrearsBillingPlannerService.BuildInitialRecurringInvoiceDraft(
                recurringBookingModel,
                productVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());
            if (draft is null)
            {
                return null;
            }

            var checkoutAmount = marketplaceBooking.ProductPricing.IsTaxInclusive
                ? marketplaceBooking.TotalAmount ?? draft.TotalAmount
                : marketplaceBooking.TotalAmountExcludeTax ?? draft.TotalAmount;

            lineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = draft.Currency.ToString().ToLowerInvariant(),
                        UnitAmountDecimal = (checkoutAmount * 100).RoundedDecimal(),
                        TaxBehavior = marketplaceBooking.ProductPricing.IsTaxInclusive ? "inclusive" : "exclusive",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            TaxCode = "txcd_10103001",
                            Name = draft.Lines.FirstOrDefault()?.Description ??
                                   productVersion.ListingMetadata?.Title ??
                                   "Subscription invoice",
                        },
                    },
                },
            ];
        }
        else
        {
            lineItems =
            [
                new SessionLineItemOptions
                {
                    Price = stripeProduct.StripePrice.StripePriceId,
                    Quantity = marketplaceBooking.Quantity,
                },
            ];
        }

        var sessionOptions = new SessionCreateOptions
        {
            Customer = args.StripeCustomerId,
            LineItems = lineItems,
            Mode = "payment",
            UiMode = "hosted_page",
            PaymentMethodTypes = ["card"],
            ClientReferenceId = recurringBooking.Id,
            SuccessUrl = checkoutReturnUrl,
            CancelUrl = checkoutReturnUrl,
            AutomaticTax = new SessionAutomaticTaxOptions
            {
                Enabled = true,
            },
            CustomerUpdate = new SessionCustomerUpdateOptions
            {
                Address = "auto",
                Shipping = "auto",
            },
        };
        var hostPaymentIntentData = hostStripeApplicationFeeService.CreateDestinationCharge(
            marketplaceBooking.ProductVersion.Product.Organization.Type,
            args.StripeConnectAccountId,
            marketplaceBooking.HostCommissionAmount);
        if (hostPaymentIntentData is not null)
        {
            sessionOptions.Customer = null;
            sessionOptions.CustomerUpdate = null;
            sessionOptions.PaymentIntentData = hostPaymentIntentData;
            UseInlineHostPriceData(sessionOptions.LineItems, marketplaceBooking, productVersion);
        }

        var session = await sessionCreateService.CreateAsync(
            sessionOptions,
            new RequestOptions
            {
                IdempotencyKey = recurringBooking.Id,
                StripeAccount = hostPaymentIntentData is null ? args.StripeConnectAccountId : null,
            },
            cancellationToken);

        var stripeCustomer =
            await repositoryFactory.StripeCustomerRepository.GetByStripeCustomerIdAsync(args.StripeCustomerId, cancellationToken) ??
            throw new StripeCustomerNotFound();
        var stripeCheckoutSession = new StripeCheckoutSession
        {
            Id = randomHelper.Generate(),
            StripeCheckoutSessionId = session.Id,
            CheckoutUrl = session.Url,
            PaymentIntentId = session.PaymentIntentId,
            ChargeType = hostPaymentIntentData is null ? "Direct" : "Destination",
            StripeAccountId = hostPaymentIntentData is null ? args.StripeConnectAccountId : null,
            DestinationAccountId = hostPaymentIntentData?.TransferData?.Destination,
            StripeCustomer = stripeCustomer,
        };

        stripeCheckoutSession = repositoryFactory.StripeCheckoutSessionRepository.Add(stripeCheckoutSession);
        marketplaceBooking.StripeCheckoutSession = stripeCheckoutSession;
        marketplaceBooking.PaymentStatus = session.PaymentStatus switch
        {
            "no_payment_required" => PaymentStatusConstants.NoPaymentRequired,
            "unpaid" => PaymentStatusConstants.Pending,
            "paid" => PaymentStatusConstants.Confirmed,
            _ => throw new ArgumentOutOfRangeException(null,
                "Unexpected value encountered. Update enum mapping or caller input to include this case."),
        };

        _ = repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForMarketplaceBookingAsync(
            marketplaceBooking.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (recurringBooking.MarketplaceBookingSubscription is not null)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                recurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }

        return new CreateCheckoutSessionAsyncResponse(marketplaceBooking.PaymentStatus);
    }

    private static void UseInlineHostPriceData(
        IEnumerable<SessionLineItemOptions> lineItems,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productVersion.Currency);
        foreach (var lineItem in lineItems.Where(item => item.PriceData is null))
        {
            lineItem.Price = null;
            lineItem.PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = productVersion.Currency.ToLowerInvariant(),
                UnitAmountDecimal = (marketplaceBooking.ProductPricing.Price * 100m).RoundedDecimal(),
                TaxBehavior = marketplaceBooking.ProductPricing.IsTaxInclusive ? "inclusive" : "exclusive",
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    TaxCode = "txcd_10103001",
                    Name = marketplaceBooking.ProductPricing.ListingMetadata.Title ??
                           productVersion.ListingMetadata?.Title ??
                           "Host booking",
                },
            };
        }
    }
}
