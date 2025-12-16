using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Core.V1;
using Core.Api.Mappers;
using Core.Api.Services;
using Enterprise.Shared.FileStorage;
using Enterprise.Shared.Version;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Core.V1.Version;

namespace Core.Api.Controllers;

[ApiController]
public class CoreController(
    IVersionService versionService,
    CoreConfiguration coreConfiguration,
    IFileUploaderService fileUploaderService,
    ICdnService cdnService,
    IPrivateFileService privateFileService,
    IMapper mapper,
    ITopicEventSender topicEventSender)
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

    public override async Task<IActionResult> RaiseGraphqlChange(
        string topicName,
        string id,
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        CancellationToken cancellationToken = default)
    {
        if (x_API_Key != coreConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        return Ok();
    }

    public override async Task<ActionResult<FileUploadResponse>> UploadPublicAccessFile(IFormFile file, CancellationToken cancellationToken = default)
    {
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);

        return mapper.MapTo(
            await fileUploaderService.UploadToCdnAsync(memoryStream, file.ContentType, Path.GetExtension(file.FileName), false, cancellationToken));
    }

    public override async Task<IActionResult> GetPublicCdnFile(string filename, CancellationToken cancellationToken = default)
    {
        var (exists, contentType, content) = await cdnService.GetAsync(filename, cancellationToken);
        return exists ? File(content, contentType) : NotFound();
    }

    public override async Task<ActionResult<FileUploadResponse>> UploadPrivateAccessFile(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);

        return mapper.MapTo(
            await fileUploaderService.UploadToPrivateStorageAsync(
                memoryStream,
                file.ContentType,
                Path.GetExtension(file.FileName),
                false,
                cancellationToken));
    }

    public override async Task<IActionResult> GetPrivateFile(string filename, CancellationToken cancellationToken = default)
    {
        var (exists, contentType, content) = await privateFileService.GetAsync(filename, cancellationToken);
        return exists ? File(content, contentType) : NotFound();
    }
}
