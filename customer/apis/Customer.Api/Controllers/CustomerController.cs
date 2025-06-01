using Api.Shared.Services.OpenApi.Skedular.Customer.V1;
using Customer.Api.Services;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Customer.V1.Version;

namespace Customer.Api.Controllers;

[ApiController]
public class CustomerController(IVersionService versionService, IWorkaroundService workaroundService) : CustomerControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<IActionResult> Republish(string customerId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishCustomerAsync(customerId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllCustomersAsync(cancellationToken);

        return Ok();
    }
}
