using Api.Shared.Services.OpenApi.Skedular.BookingWorkaround.V1;
using Booking.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
public class BookingWorkaroundController(IWorkaroundService workaroundService) : BookingWorkaroundControllerBase
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

    public override async Task<IActionResult> GenerateAllLocationsResourcesSlots(CancellationToken cancellationToken = default)
    {
        await workaroundService.GenerateAllLocationsResourcesSlotsAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> GenerateLocationResourcesSlots(string locationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.GenerateLocationResourcesSlotsAsync(locationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> GenerateOrganizationArrearsInvoices(
        string organizationId,
        CancellationToken cancellationToken = default)
    {
        await workaroundService.GenerateOrganizationArrearsInvoicesAsync(organizationId, cancellationToken);

        return Ok();
    }
}
