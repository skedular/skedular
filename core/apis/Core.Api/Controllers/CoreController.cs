using Api.Shared.Services.OpenApi.Skedular.Core.V1;
using Core.Api.Services;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Core.V1.Version;

namespace Core.Api.Controllers;

[ApiController]
public class CoreController(IVersionService versionService, IFileUploaderService fileUploaderService, ICdnService cdnService) : CoreControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<ActionResult<FileUploadResponse>> UploadPublicAccessFile(IFormFile file, CancellationToken cancellationToken = default)
    {
        var uploadResponse =
            await fileUploaderService.UploadAsync(file.OpenReadStream(), file.ContentType, Path.GetExtension(file.FileName), cancellationToken);

        return new FileUploadResponse { Id = uploadResponse.Item1, CdnUrl = uploadResponse.Item2.ToString() };
    }

    public override async Task<IActionResult> GetPublicCdnFile(string filename, CancellationToken cancellationToken = default)
    {
        var (exists, contentType, content) = await cdnService.GetAsync(filename, cancellationToken);
        return exists ? File(content, contentType) : NotFound();
    }
}
