using Api.Shared.Services.OpenApi.Skedular.Organization.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Organization.Api.Services;
using Version = Api.Shared.Services.OpenApi.Skedular.Organization.V1.Version;

namespace Organization.Api.Controllers;

[ApiController]
public class OrganizationController(
    IVersionService versionService,
    IWorkaroundService workaroundService,
    IAzureTenantService azureTenantService,
    IOrganizationSsoService organizationSsoService,
    IPaymentService paymentService) : OrganizationControllerBase
{
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
}
