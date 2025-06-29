using Api.Shared.Services.OpenApi.Skedular.Core.V1;
using Core.Api.Mappers;
using Core.Api.Services;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Core.V1.Version;

namespace Core.Api.Controllers;

[ApiController]
public class CoreController(IVersionService versionService, IFileUploaderService fileUploaderService, ICdnService cdnService, IMapper mapper)
    : CoreControllerBase
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
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);

        return mapper.MapTo(
            await fileUploaderService.UploadAsync(memoryStream, file.ContentType, Path.GetExtension(file.FileName), cancellationToken));
    }

    public override async Task<IActionResult> GetPublicCdnFile(string filename, CancellationToken cancellationToken = default)
    {
        var (exists, contentType, content) = await cdnService.GetAsync(filename, cancellationToken);
        return exists ? File(content, contentType) : NotFound();
    }
}
