using Api.Shared.Services.OpenApi.UnityHub.Billing.V1;
using Billing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Billing.Api.Controllers;

[ApiController]
public class BillingController(IWorkaroundService workaroundService) : BillingControllerBase
{
    public override async Task<IActionResult> RepublishOrganizationBillingInfo(
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishOrganizationBillingInfoAsync(organizationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAllOrganizationsBillingInfo(
        CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationsBillingInfoAsync(cancellationToken);

        return Ok();
    }
}
