using Api.Shared.Services.OpenApi.Skedular.Location.V1;
using Enterprise.Shared.Version;
using Location.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Location.V1.Version;

namespace Location.Api.Controllers;

[ApiController]
public class LocationController(IVersionService versionService, IWorkaroundService workaroundService) : LocationControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<IActionResult> Republish(string locationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishLocationAsync(locationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllLocationsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RegenerateAllDailyAnalytics(CancellationToken cancellationToken = default)
    {
        await workaroundService.RegenerateAllDailyAnalyticsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RegenerateDailyAnalytics(string locationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RegenerateDailyAnalyticsAsync(locationId, cancellationToken);

        return Ok();
    }
}
