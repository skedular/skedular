using Api.Shared.Services.OpenApi.Skedular.Booking.V1;
using Booking.Api.Services;
using Booking.Shared.Services;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Booking.V1.Version;

namespace Booking.Api.Controllers;

[ApiController]
public class BookingController(
    IVersionService versionService,
    IWorkaroundService workaroundService,
    IResourceBookingSlotsHelperService resourceBookingSlotsHelperService)
    : BookingControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

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

    public override async Task<IActionResult> RepublishResourcesSlots(string resourceId, CancellationToken cancellationToken = default)
    {
        await resourceBookingSlotsHelperService.GenerateAsync(resourceId, cancellationToken);

        return Ok();
    }
}
