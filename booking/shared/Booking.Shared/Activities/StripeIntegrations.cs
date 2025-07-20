using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Stripe;
using Stripe.Checkout;
using Temporalio.Activities;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using StripeCustomer = Booking.Shared.Database.Entities.StripeCustomer;

namespace Booking.Shared.Activities;

public class StripeIntegrations(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IStripeProductPricingService stripeProductPricingService,
    IStripeCustomerService stripeCustomerService,
    ICreatable<Session, SessionCreateOptions> sessionCreateService,
    IRandomHelper randomHelper,
    IMapper mapper)
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

        var productVersionIds = booking.LineItems.Select(item => item.ProductVersionId).Distinct().ToList();
        var productVersions = await repositoryFactory.ProductVersionRepository.GetByIdsAsync(productVersionIds, cancellationToken);
        if (productVersions.Count != productVersionIds.Count)
        {
            throw new InvalidOperationException();
        }

        var stripeConnectAccountConnection = await organizationServiceClient.Admin_GetStripeConnectAccountsAsync(
            new Admin_GetStripeConnectAccountsInput
            {
                After = string.Empty,
                First = ((int?)null).ToNullInt(),
                Before = string.Empty,
                Last = ((int?)null).ToNullInt(),
                Where = new StripeConnectAccountWhereInput
                {
                    OrganizationId = productVersions.First().Product.Organization.Id, OnboardingCompleted = true
                }
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        var stripeConnectAccountId = stripeConnectAccountConnection.Edges.Select(item => item.Node).First(item => item.IsDefault).StripeAccountId;

        foreach (var productVersion in productVersions)
        {
            if (productVersion.StripeProduct is not null && productVersion.StripePrice is not null)
            {
                continue;
            }

            var (stripeProduct, stripePrice) = await stripeProductPricingService.UpsertProductPricingAsync(
                mapper.MapTo(productVersion),
                productVersion,
                stripeConnectAccountId,
                cancellationToken);

            productVersion.StripeProduct = stripeProduct;
            productVersion.StripePrice = stripePrice;
            _ = repositoryFactory.ProductVersionRepository.Update(productVersion);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new UpsertProductAndPricingResponse(stripeConnectAccountId);
    }

    [Activity]
    public async Task<UpsertBookingRelatedStripeCustomerResponse?> UpsertBookingRelatedStripeCustomerAsync(
        UpsertBookingRelatedStripeCustomerInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted())
        {
            return null;
        }

        StripeCustomer stripeCustomer;
        if (booking.PaidByCustomer is not null)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(booking.PaidByCustomer.Id, true, cancellationToken) ??
                           throw new CustomerNotFound();
            stripeCustomer = await stripeCustomerService.AddCustomerAsync(customer, args.StripeConnectAccountId, cancellationToken);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (booking.PaidByOrganization is not null)
        {
            var organization =
                await repositoryFactory.OrganizationRepository.GetByIdAsync(booking.PaidByOrganization.Id, false, false, cancellationToken) ??
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

        var productVersionIds = booking.LineItems.Select(item => item.ProductVersionId).Distinct().ToList();
        var productVersions = await repositoryFactory.ProductVersionRepository.GetByIdsAsync(productVersionIds, cancellationToken);
        var lineItems = booking.LineItems.SelectMany(item =>
        {
            var productVersion = productVersions.First(productVersion => productVersion.Id == item.ProductVersionId);

            return booking.Schedules.Select(schedule => new SessionLineItemOptions
            {
                Price = productVersion.StripePrice!.StripePriceId,
                Quantity = productVersion.PriceUnit switch
                {
                    PriceUnitConstants.PerMinute => Convert.ToInt32((schedule.Until - schedule.From).TotalMinutes) * item.Quantity,
                    PriceUnitConstants.PerHour => Convert.ToInt32((schedule.Until - schedule.From).TotalHours) * item.Quantity,
                    PriceUnitConstants.PerUse => item.Quantity,
                    _ => throw new ArgumentOutOfRangeException()
                }
            });
        }).ToList();

        if (booking.StripeCheckoutSession is not null)
        {
            return new CreateCheckoutSessionAsyncResponse(booking.PaymentStatus);
        }

        var session = await sessionCreateService.CreateAsync(
            new SessionCreateOptions
            {
                Customer = args.StripeCustomerId,
                LineItems = lineItems,
                Mode = "payment",
                UiMode = "hosted",
                PaymentMethodTypes = ["card"],
                ClientReferenceId = booking.Id,
                SuccessUrl = applicationConfiguration.WebAppBaseDomain.ToString(),
                CancelUrl = applicationConfiguration.WebAppBaseDomain.ToString(),
                AutomaticTax = new SessionAutomaticTaxOptions { Enabled = true },
                CustomerUpdate = new SessionCustomerUpdateOptions { Address = "auto", Shipping = "auto" }
            },
            new RequestOptions { IdempotencyKey = booking.Id, StripeAccount = args.StripeConnectAccountId },
            cancellationToken);

        var stripeCustomer =
            await repositoryFactory.StripeCustomerRepository.GetByStripeCustomerIdAsync(args.StripeCustomerId, cancellationToken) ??
            throw new StripeCustomerNotFound();
        var stripeCheckoutSession = new StripeCheckoutSession
        {
            Id = randomHelper.Generate(), StripeCheckoutSessionId = session.Id, CheckoutUrl = session.Url, StripeCustomer = stripeCustomer
        };

        stripeCheckoutSession = repositoryFactory.StripeCheckoutSessionRepository.Add(stripeCheckoutSession);
        booking.StripeCheckoutSession = stripeCheckoutSession;
        booking.PaymentStatus = session.PaymentStatus switch
        {
            "no_payment_required" => PaymentStatusConstants.NoPaymentRequired,
            "unpaid" => PaymentStatusConstants.Pending,
            "paid" => PaymentStatusConstants.Confirmed,
            _ => throw new ArgumentOutOfRangeException()
        };

        _ = repositoryFactory.BookingRepository.Update(booking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCheckoutSessionAsyncResponse(booking.PaymentStatus);
    }
}
