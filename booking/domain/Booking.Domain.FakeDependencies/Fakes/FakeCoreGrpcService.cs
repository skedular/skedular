using Api.Shared.Grpc.Skedular.Core.Core.V1;
using Grpc.Core;

namespace Booking.Domain.FakeDependencies.Fakes;

public class FakeCoreGrpcService(FakeCoreGrpcState state) : CoreService.CoreServiceBase
{
    public override async Task<FileUploadResponse> Admin_UploadToPrivateStorage(
        IAsyncStreamReader<UploadFileRequest> requestStream,
        ServerCallContext context)
    {
        var contentType = string.Empty;
        string? extension = null;
        await using var uploadStream = new MemoryStream();

        await foreach (var message in requestStream.ReadAllAsync(context.CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(extension) && !string.IsNullOrWhiteSpace(message.Extension))
            {
                extension = message.Extension;
            }

            if (string.IsNullOrWhiteSpace(contentType) && !string.IsNullOrWhiteSpace(message.ContentType))
            {
                contentType = message.ContentType;
            }

            if (message.Chunk is not null && message.Chunk.Length > 0)
            {
                await uploadStream.WriteAsync(message.Chunk.Memory, context.CancellationToken);
            }
        }

        state.UploadToPrivateStorageRequests.Enqueue(
            new RecordedUploadToPrivateStorageRequest(
                DateTimeOffset.UtcNow,
                extension,
                string.IsNullOrWhiteSpace(contentType) ? null : contentType,
                uploadStream.ToArray()));

        return state.UploadToPrivateStorageResponse;
    }
}
