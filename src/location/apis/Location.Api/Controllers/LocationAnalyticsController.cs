using Api.Shared.Services.OpenApi.Skedular.Location.Analytics.V1;
using Location.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Location.Api.Controllers;

[ApiController]
public class LocationAnalyticsController(IWorkaroundService workaroundService) : LocationAnalyticsControllerBase
{
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

    public override async Task<IActionResult> RegenerateResourceAvailabilitySnapshots(string locationId,
        RegenerateResourceAvailabilitySnapshotsInput body, CancellationToken cancellationToken = default)
    {
        await workaroundService.RegenerateResourceAvailabilitySnapshotsAsync(locationId, body, cancellationToken);

        return Ok();
    }
}
