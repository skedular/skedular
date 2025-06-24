using System.Globalization;
using Api.Shared.Services.OpenApi.Skedular.Organization.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Organization.Api.Services;
using Organization.Shared.Publishers;
using Stripe;
using StripeConfiguration = Enterprise.Shared.Payment.Configurations.StripeConfiguration;
using Version = Api.Shared.Services.OpenApi.Skedular.Organization.V1.Version;

namespace Organization.Api.Controllers;

[ApiController]
public class OrganizationController(
    IVersionService versionService,
    StripeConfiguration stripeConfiguration,
    IWorkaroundService workaroundService,
    IAzureTenantService azureTenantService,
    IOrganizationSsoService organizationSsoService,
    IPaymentService paymentService,
    IOrganizationInternalPublisher organizationInternalPublisher,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    TimeProvider timeProvider,
    ILogger<OrganizationController> logger) : OrganizationControllerBase
{
    private static readonly string s_homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<IActionResult> Republish(string organizationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishOrganizationAsync(organizationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> AzureTenantAdminConsentUrl(CancellationToken cancellationToken = default) =>
        Redirect(await azureTenantService.GenerateAdminConsentUrlAsync(cancellationToken));

    public override async Task<IActionResult> OnboardAzureTenant(
        string tenant,
        // ReSharper disable once InconsistentNaming
        bool admin_consent,
        string state,
        string? error,
        // ReSharper disable once InconsistentNaming
        string? error_description,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            throw new InvalidOperationException($"Azure tenant onboarding went wrong with error {error} and message {error_description}.");
        }

        var redirectUri = await azureTenantService.InstallAsync(tenant, state, cancellationToken);

        return Redirect(redirectUri.AbsoluteUri);
    }

    public override async Task<IActionResult> SsoAcs(CancellationToken cancellationToken = default)
    {
        if (!Request.Form.ContainsKey("SAMLResponse"))
        {
            throw new ArgumentException("SAMLResponse is required.");
        }

        var rawSamlResponse = Request.Form["SAMLResponse"].ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(rawSamlResponse);

        if (!Request.Form.ContainsKey("RelayState"))
        {
            throw new ArgumentException("RelayState is required.");
        }

        var redirectUrl = Request.Form["RelayState"].ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(redirectUrl);

        await organizationSsoService.ProcessSsoResponseAsync(Response, rawSamlResponse, cancellationToken);

        return Redirect(redirectUrl);
    }

    public override async Task<IActionResult> AddPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(await paymentService.HandleStripePaymentMethodEventAsync(
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

            if (stripeConfiguration.LogStripPlatformAccountWebhookMessages)
            {
                var tempFileDirectoryPath = Path.Combine(s_homeDirectory, "stripe-logs/platform");
                Directory.CreateDirectory(tempFileDirectoryPath);
                var tempFilePath = Path.Combine(tempFileDirectoryPath,
                    $"{timeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)}.json");
                await System.IO.File.WriteAllTextAsync(tempFilePath, json, cancellationToken);
                logger.LogInformation("Stripe Platform account event JSON logged to file: {FilePath}", tempFilePath);
            }

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

            if (stripeConfiguration.LogStripeConnectAccountWebhookMessages)
            {
                var tempFileDirectoryPath = Path.Combine(s_homeDirectory, "stripe-logs/connect");
                Directory.CreateDirectory(tempFileDirectoryPath);
                var tempFilePath = Path.Combine(tempFileDirectoryPath,
                    $"{timeProvider.GetUtcNow().ToString("o", CultureInfo.InvariantCulture)}.json");
                await System.IO.File.WriteAllTextAsync(tempFilePath, json, cancellationToken);
                logger.LogInformation("Stripe Connect account event JSON logged to file: {FilePath}", tempFilePath);
            }

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
                        await organizationInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                            stripeEvent.Account,
                            json,
                            cancellationToken);
                    }
                    break;

                case EventTypes.AccountExternalAccountCreated:
                case EventTypes.AccountExternalAccountDeleted:
                case EventTypes.AccountExternalAccountUpdated:
                    await organizationInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                        stripeEvent.Account,
                        json,
                        cancellationToken);

                    break;

                case EventTypes.AccountUpdated:
                    {
                        var stripeAccount = stripeEvent.Data.Object as Account;
                        ArgumentNullException.ThrowIfNull(stripeAccount);

                        await organizationInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(
                            stripeAccount.Id,
                            json,
                            cancellationToken);
                    }

                    break;

                case EventTypes.CustomerCreated:
                case EventTypes.CustomerUpdated:
                case EventTypes.CustomerDeleted:
                    var customer = stripeEvent.Data.Object as Customer;
                    ArgumentNullException.ThrowIfNull(customer);

                    await organizationInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(customer.Id, json, cancellationToken);
                    break;

                case EventTypes.ChargePending:
                case EventTypes.ChargeSucceeded:
                case EventTypes.ChargeUpdated:
                case EventTypes.ChargeExpired:
                case EventTypes.ChargeFailed:
                    var charge = stripeEvent.Data.Object as Charge;
                    ArgumentNullException.ThrowIfNull(charge);

                    await organizationInternalPublisher.PublishStripeConnectAccountWebhookEventReceivedAsync(charge.Id, json, cancellationToken);
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
}
