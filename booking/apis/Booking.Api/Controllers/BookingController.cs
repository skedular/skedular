using Api.Shared.Services.OpenApi.Skedular.Booking.V1;
using Booking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
public class TeamController(IWorkaroundService workaroundService) : BookingControllerBase
{
    public override async Task<IActionResult> Republish(string bookingId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishBookingAsync(bookingId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllBookingsAsync(cancellationToken);

        return Ok();
    }
}
