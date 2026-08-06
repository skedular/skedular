using Api.Shared.Grpc.Skedular.Core.Core.V1;
using Api.Shared.Services.Configurations.Grpc;
using Core.Api.Mappers;
using Core.Api.Services;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Version;
using Grpc.Core;
using Version = Api.Shared.Grpc.Skedular.Core.Core.V1.Version;

namespace Core.Api.Grpc;

public class CoreGrpcService(
    IGrpcAuthenticator grpcAuthenticator,
    CoreConfiguration coreConfiguration,
    IVersionService versionService,
    IFileUploaderService fileUploaderService,
    IGrpcMapper grpcMapper) : CoreService.CoreServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version
        {
            Major = version.Major,
            Minor = version.Minor,
            Build = version.Build,
            Revision = version.Revision,
        });
    }

    public override async Task<FileUploadResponse> Admin_UploadToPrivateStorage(
        IAsyncStreamReader<UploadFileRequest> requestStream,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(coreConfiguration.ApiKey);

        var contentType = string.Empty;
        string? extension = null;

        await using var uploadStream = new MemoryStream();
        await foreach (var message in requestStream.ReadAllAsync(context.CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(contentType) && !string.IsNullOrWhiteSpace(message.ContentType))
            {
                contentType = message.ContentType;
            }

            if (string.IsNullOrWhiteSpace(extension) && !string.IsNullOrWhiteSpace(message.Extension))
            {
                extension = message.Extension;
            }

            if (message.Chunk is not null && message.Chunk.Length > 0)
            {
                await uploadStream.WriteAsync(message.Chunk.Memory, context.CancellationToken);
            }
        }

        uploadStream.Seek(0, SeekOrigin.Begin);

        return grpcMapper.MapToGrpcResponse(
            await fileUploaderService.UploadToPrivateStorageAsync(uploadStream, contentType, extension, true, context.CancellationToken));
    }
}
