using Api.Shared.Services.OpenApi.Skedular.Location.V1;
using Location.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Location.Api.Controllers;

[ApiController]
public class LocationController(IWorkaroundService workaroundService) : LocationControllerBase
{
    public override async Task<IActionResult>
        Republish(string locationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishLocationAsync(locationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllLocationsAsync(cancellationToken);

        return Ok();
    }
}
