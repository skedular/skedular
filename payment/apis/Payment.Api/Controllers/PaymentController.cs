using Api.Shared.Services.OpenApi.Skedular.Payment.V1;
using Microsoft.AspNetCore.Mvc;
using Payment.Api.Services;
using Stripe;
using StripeConfiguration = Payment.Shared.Configurations.StripeConfiguration;

namespace Payment.Api.Controllers;

[ApiController]
public class PaymentController(
    StripeConfiguration stripeConfiguration,
    IWorkaroundService workaroundService,
    IOrganizationPaymentService organizationPaymentService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService) : PaymentControllerBase
{
    public override async Task<IActionResult> AddOrganizationPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(await organizationPaymentService.AddPaymentMethodAsync(
            setup_intent,
            setup_intent_client_secret,
            redirect_status,
            cancellationToken));

    public override async Task<IActionResult> RefreshOrganizationStripeConnectAccountOnboarding(
        string code,
        CancellationToken cancellationToken = default)
    {
        var onboardingUrl = await organizationStripeConnectAccountService.GetNewOnboardingUrlAsync(code, cancellationToken);
        return Redirect(onboardingUrl);
    }

    public override async Task<IActionResult> OrganizationStripeConnectAccountOnboardingCompleted(
        // ReSharper disable once InconsistentNaming
        string? stripe_Signature,
        string? body,
        CancellationToken cancellationToken = default)
    {
        _ = EventUtility.ParseEvent(body);
        var stripeEvent = EventUtility.ConstructEvent(body, stripe_Signature, stripeConfiguration.SecretKey);
        switch (stripeEvent.Type)
        {
            case EventTypes.AccountUpdated:
                var stripeAccount = stripeEvent.Data.Object as Account;
                ArgumentNullException.ThrowIfNull(stripeAccount);

                await organizationStripeConnectAccountService.CompleteOnboardAsync(stripeAccount, cancellationToken);

                break;
        }

        return Ok();
    }

    public override async Task<IActionResult> RepublishAllOrganizationStripeConnectAccounts(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationStripeConnectAccountsAsync(cancellationToken);

        return Ok();
    }
}
