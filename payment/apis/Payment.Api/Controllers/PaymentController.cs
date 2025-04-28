using Api.Shared.Services.OpenApi.Skedular.Payment.V1;
using Microsoft.AspNetCore.Mvc;
using Payment.Api.Services;
using Payment.Shared.Publishers;
using Stripe;
using Stripe.Checkout;
using StripeConfiguration = Payment.Shared.Configurations.StripeConfiguration;

namespace Payment.Api.Controllers;

[ApiController]
public class PaymentController(
    StripeConfiguration stripeConfiguration,
    IWorkaroundService workaroundService,
    IOrganizationPaymentService organizationPaymentService,
    ICustomerPaymentService customerPaymentService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IPaymentInternalPublisher paymentInternalPublisher,
    ILogger<PaymentController> logger) : PaymentControllerBase
{
    public override async Task<IActionResult> AddOrganizationPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(await organizationPaymentService.HandleStripePaymentMethodEventAsync(
            setup_intent,
            setup_intent_client_secret,
            redirect_status,
            cancellationToken));

    public override async Task<IActionResult> AddCustomerPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(await customerPaymentService.HandleStripePaymentMethodEventAsync(
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

    public override async Task<IActionResult> ProcessStripePlatformAccountEvent(
        // ReSharper disable once InconsistentNaming
        string? stripe_Signature,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);
            _ = EventUtility.ConstructEvent(json, stripe_Signature, stripeConfiguration.PlatformAccountWebhookKey, throwOnApiVersionMismatch: false);

            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Failed to process Stripe Platform event.");

            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process Stripe Platform event.");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public override async Task<IActionResult> ProcessStripeConnectAccountEvent(
        // ReSharper disable once InconsistentNaming
        string? stripe_Signature,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(cancellationToken);
            var stripeEvent = EventUtility.ConstructEvent(
                json, 
                stripe_Signature, 
                stripeConfiguration.ConnectAccountWebhookKey,
                throwOnApiVersionMismatch: false);
            switch (stripeEvent.Type)
            {
                case EventTypes.AccountApplicationAuthorized:
                case EventTypes.AccountApplicationDeauthorized:
                    {
                        await paymentInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                            stripeEvent.Account,
                            json,
                            cancellationToken);
                    }
                    break;

                case EventTypes.AccountExternalAccountCreated:
                case EventTypes.AccountExternalAccountDeleted:
                case EventTypes.AccountExternalAccountUpdated:
                case EventTypes.AccountUpdated:
                    {
                        var stripeAccount = stripeEvent.Data.Object as Account;
                        ArgumentNullException.ThrowIfNull(stripeAccount);

                        await paymentInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                            stripeAccount.Id,
                            json,
                            cancellationToken);
                    }

                    break;

                case EventTypes.CheckoutSessionCompleted:
                case EventTypes.CheckoutSessionExpired:
                case EventTypes.CheckoutSessionAsyncPaymentSucceeded:
                case EventTypes.CheckoutSessionAsyncPaymentFailed:
                    var session = stripeEvent.Data.Object as Session;
                    ArgumentNullException.ThrowIfNull(session);
                    break;
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Failed to process Stripe event.");

            return BadRequest();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process Stripe event.");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    public override async Task<IActionResult> RepublishAllOrganizationStripeConnectAccounts(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationStripeConnectAccountsAsync(cancellationToken);

        return Ok();
    }
}
