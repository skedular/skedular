using Api.Shared.Services.OpenApi.Skedular.Notification.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Notification.V1.Version;

namespace Notification.Api.Controllers;

[ApiController]
public class NotificationController(IVersionService versionService) : NotificationControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }
}
