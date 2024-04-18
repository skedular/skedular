using Api.Shared.Services.OpenApi.UnityHub.Team.V1;
using Booking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
public class TeamController(IWorkaroundService workaroundService) : TeamControllerBase
{
    public override async Task<IActionResult> Republish(string teamId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishBookingAsync(teamId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllBookingsAsync(cancellationToken);

        return Ok();
    }
}
