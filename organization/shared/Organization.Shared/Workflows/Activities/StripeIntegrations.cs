using Enterprise.Shared.Configurations;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Flurl;
using Organization.Shared.Mappers;
using Organization.Shared.Repositories;
using Stripe;
using Temporalio.Activities;

namespace Organization.Shared.Workflows.Activities;

public record SetOrganizationPaymentMethodInput(string OrganizationId, string SetupIntentId, string RedirectStatus);

public class StripeIntegrations(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IRandomHelper randomHelper,
    IRetrievable<SetupIntent, SetupIntentGetOptions> setupIntentRetrievableService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService)
{
    [Activity]
    public async Task<string> SetOrganizationPaymentMethodAsync(SetOrganizationPaymentMethodInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(args.OrganizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        var redirectUrl = Url.Combine(applicationConfiguration.WebAppBaseDomain, "organizations", organization!.Id, "admin");

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

            var stripePaymentMethod = mapper.MapTo(paymentMethod, args.SetupIntentId, organization);
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
