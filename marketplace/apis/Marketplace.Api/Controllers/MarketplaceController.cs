using Api.Shared.Services.OpenApi.Skedular.Marketplace.V1;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

[ApiController]
public class MarketplaceController : MarketplaceControllerBase
{
    public override Task<IActionResult> RepublishAllOrganizationProducts(string organizationId, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public override Task<IActionResult> RepublishAllProducts(CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
