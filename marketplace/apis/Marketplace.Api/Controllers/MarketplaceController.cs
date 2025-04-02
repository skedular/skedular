using Api.Shared.Services.OpenApi.Skedular.Marketplace.V1;
using Marketplace.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

[ApiController]
public class MarketplaceController(IWorkaroundService workaroundService) : MarketplaceControllerBase
{
    public override async Task<IActionResult> RepublishAllOrganizationProducts(string organizationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationProductsAsync(organizationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAllProducts(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllProductsAsync(cancellationToken);

        return Ok();
    }
}
