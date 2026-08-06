using Api.Shared.Services.OpenApi.Skedular.Booking.Core.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Booking.Core.V1.Version;

namespace Booking.Api.Controllers;

[ApiController]
public class BookingCoreController(IVersionService versionService) : BookingCoreControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major,
            Minor = version.Minor,
            Build = version.Build,
            Revision = version.Revision,
        });
    }
}
