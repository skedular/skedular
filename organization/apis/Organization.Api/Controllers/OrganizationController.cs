using Api.Shared.Services.OpenApi.UnityHub.Organization.V1;
using Microsoft.AspNetCore.Mvc;
using Organization.Api.Services;

namespace Organization.Api.Controllers;

[ApiController]
public class OrganizationController(IWorkaroundService workaroundService) : OrganizationControllerBase
{
    public override async Task<IActionResult>
        Republish(string organizationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishOrganizationAsync(organizationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationsAsync(cancellationToken);

        return Ok();
    }
}
