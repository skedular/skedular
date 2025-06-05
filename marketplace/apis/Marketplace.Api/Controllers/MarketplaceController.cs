using Api.Shared.Services.OpenApi.Skedular.Marketplace.V1;
using Enterprise.Shared.Version;
using Marketplace.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Marketplace.V1.Version;

namespace Marketplace.Api.Controllers;

[ApiController]
public class MarketplaceController(
    IVersionService versionService,
    IWorkaroundService workaroundService,
    IFileUploaderService fileUploaderService) : MarketplaceControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
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

    public override async Task<ActionResult<FileUploadResponse>> UploadFile(IFormFile file, CancellationToken cancellationToken = default) =>
        new FileUploadResponse { Id = await fileUploaderService.UploadAsync(file.OpenReadStream(), file.ContentType, cancellationToken) };
}
