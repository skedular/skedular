using Api.Shared.Services.OpenApi.Skedular.Marketplace.Core.V1;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Marketplace.Core.V1.Version;

namespace Marketplace.Api.Controllers;

[ApiController]
public class MarketplaceCoreController(IVersionService versionService) : MarketplaceCoreControllerBase
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
