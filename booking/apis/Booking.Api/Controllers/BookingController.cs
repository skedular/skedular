using Api.Shared.Services.OpenApi.Skedular.Booking.V1;
using Booking.Api.Services;
using Booking.Shared.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
public class BookingController(IWorkaroundService workaroundService, IResourceBookingSlotsHelperService resourceBookingSlotsHelperService)
    : BookingControllerBase
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

    public override async Task<IActionResult> RepublishAllResourcesSlots(CancellationToken cancellationToken = default)
    {
        await resourceBookingSlotsHelperService.GenerateAllAsync(cancellationToken);

        return Ok();
    }
}
