using Api.Shared.Services.OpenApi.Skedular.Core.V1;
using Core.Api.Services;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Core.V1.Version;

namespace Core.Api.Controllers;

[ApiController]
public class CoreController(IVersionService versionService, IFileUploaderService fileUploaderService) : CoreControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<ActionResult<FileUploadResponse>> UploadFile(IFormFile file, CancellationToken cancellationToken = default)
    {
        var uploadResponse = await fileUploaderService.UploadAsync(file.OpenReadStream(), file.ContentType, cancellationToken);

        return new FileUploadResponse { Id = uploadResponse.Item1, CdnUrl = uploadResponse.Item2.ToString() };
    }
}
