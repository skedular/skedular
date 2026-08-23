using Api.Shared.Services;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Random;
using Flurl;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;
using Stripe;
using Temporalio.Activities;

namespace Organization.Shared.Activities;

public record SetOrganizationPaymentMethodInput(string OrganizationId, string SetupIntentId, string RedirectStatus, string? RedirectTo = null);

public class StripeIntegrations(
    ApplicationConfiguration applicationConfiguration,
    IRepositoryFactory repositoryFactory,
    IEntityMapper entityMapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IRandomHelper randomHelper,
    ICachedOrganizationService cachedOrganizationService,
    IRetrievable<SetupIntent, SetupIntentGetOptions> setupIntentRetrievableService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService)
{
    [Activity]
    public async Task<string> SetOrganizationPaymentMethodAsync(SetOrganizationPaymentMethodInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               args.OrganizationId,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        var baseRedirectUrl = IsValidRedirectUrl(args.RedirectTo)
            ? args.RedirectTo!
            : Url.Combine(applicationConfiguration.WebAppBaseDomain.ToString(), "organizations", organization.CustomDomain, "admin");

        var redirectUrl = baseRedirectUrl;

        redirectUrl = redirectUrl
            .SetQueryParam("section", "setup")
            .SetQueryParam("profileSection", "plan");

        if (args.RedirectStatus == "succeeded")
        {
            redirectUrl = redirectUrl.SetQueryParam("add-payment-method-status", "added");

            var setupIntent = await setupIntentRetrievableService.GetAsync(args.SetupIntentId, cancellationToken: cancellationToken);
            ArgumentNullException.ThrowIfNull(setupIntent);
            ArgumentNullException.ThrowIfNull(setupIntent.PaymentMethodId);

            var paymentMethod = await paymentMethodRetrievableService.GetAsync(setupIntent.PaymentMethodId, cancellationToken: cancellationToken);
            ArgumentNullException.ThrowIfNull(paymentMethod);
            ArgumentNullException.ThrowIfNull(paymentMethod.Card);

            var organizationStripePaymentMethod = entityMapper.MapTo(paymentMethod, args.SetupIntentId, organization);
            organizationStripePaymentMethod.Id = randomHelper.Generate();
            repositoryFactory.OrganizationStripePaymentMethodRepository.Add(organizationStripePaymentMethod);
            organizationOutboxPublisher.PublishOrganizations([entityMapper.MapTo(organization)], repositoryFactory.UnitOfWork);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
        }
        else
        {
            redirectUrl = redirectUrl.SetQueryParam("add-payment-method-status", "failed");
        }

        return redirectUrl;
    }

    private static bool IsValidRedirectUrl(string? url) =>
        !string.IsNullOrWhiteSpace(url) &&
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
}
