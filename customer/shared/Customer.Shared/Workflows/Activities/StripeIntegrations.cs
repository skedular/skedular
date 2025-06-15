using Api.Shared.Services;
using Customer.Shared.Mappers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Random;
using Flurl;
using Stripe;
using Temporalio.Activities;

namespace Customer.Shared.Workflows.Activities;

public record SetCustomerPaymentMethodInput(string CustomerId, string SetupIntentId, string RedirectStatus);

public class StripeIntegrations(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IRandomHelper randomHelper,
    IRetrievable<SetupIntent, SetupIntentGetOptions> setupIntentRetrievableService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService)
{
    [Activity]
    public async Task<string> SetCustomerPaymentMethodAsync(SetCustomerPaymentMethodInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(args.CustomerId, cancellationToken) ?? throw new CustomerNotFound();
        var redirectUrl = Url.Combine(applicationConfiguration.WebAppBaseDomain, "me");

        redirectUrl = redirectUrl.SetQueryParam("section", "billing-payment-setup");

        if (args.RedirectStatus == "succeeded")
        {
            redirectUrl = redirectUrl.SetQueryParam("add-payment-method-status", "added");

            var setupIntent = await setupIntentRetrievableService.GetAsync(args.SetupIntentId, cancellationToken: cancellationToken);
            ArgumentNullException.ThrowIfNull(setupIntent);
            ArgumentNullException.ThrowIfNull(setupIntent.PaymentMethodId);

            var paymentMethod = await paymentMethodRetrievableService.GetAsync(setupIntent.PaymentMethodId, cancellationToken: cancellationToken);
            ArgumentNullException.ThrowIfNull(paymentMethod);
            ArgumentNullException.ThrowIfNull(paymentMethod.Card);

            var stripePaymentMethod = mapper.MapTo(paymentMethod, args.SetupIntentId, customer);
            stripePaymentMethod.Id = randomHelper.Generate();
            repositoryFactory.StripePaymentMethodRepository.Add(stripePaymentMethod);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            redirectUrl = redirectUrl.SetQueryParam("add-payment-method-status", "failed");
        }

        return redirectUrl;
    }
}
