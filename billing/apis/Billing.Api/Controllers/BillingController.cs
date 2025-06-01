using Api.Shared.Services.OpenApi.Skedular.Billing.V1;
using Billing.Api.Services;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Billing.V1.Version;

namespace Billing.Api.Controllers;

[ApiController]
public class BillingController(IVersionService versionService, IWorkaroundService workaroundService) : BillingControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<IActionResult> RepublishOrganizationBillingInfo(string organizationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishOrganizationBillingInfoAsync(organizationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAllOrganizationsBillingInfo(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationsBillingInfoAsync(cancellationToken);

        return Ok();
    }
}
