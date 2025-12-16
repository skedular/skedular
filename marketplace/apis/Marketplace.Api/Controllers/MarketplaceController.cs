using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Marketplace.V1;
using Enterprise.Shared.Version;
using HotChocolate.Subscriptions;
using Marketplace.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Marketplace.V1.Version;

namespace Marketplace.Api.Controllers;

[ApiController]
public class MarketplaceController(
    IVersionService versionService,
    MarketplaceConfiguration marketplaceConfiguration,
    IWorkaroundService workaroundService,
    ITopicEventSender topicEventSender)
    : MarketplaceControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<IActionResult> RaiseGraphqlChange(
        string topicName,
        string id,
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        CancellationToken cancellationToken = default)
    {
        if (x_API_Key != marketplaceConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAllOrganizationProducts(string organizationId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationProductsAsync(organizationId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAllProducts(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllProductsAsync(cancellationToken);

        return Ok();
    }
}
