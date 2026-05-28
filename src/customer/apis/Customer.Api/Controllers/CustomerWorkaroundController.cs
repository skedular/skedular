using Api.Shared.Services.OpenApi.Skedular.Customer.Workaround.V1;
using Customer.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers;

[ApiController]
public class CustomerWorkaroundController(IWorkaroundService workaroundService) : CustomerWorkaroundControllerBase
{
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
