using System.Collections.Concurrent;
using Api.Shared.Grpc.Skedular.Core.Core.V1;
using GrpcFile = Api.Shared.Grpc.Skedular.Core.Core.V1.File;

namespace Booking.Domain.FakeDependencies.Fakes;

public class FakeCoreGrpcState
{
    public ConcurrentQueue<RecordedUploadToPrivateStorageRequest> UploadToPrivateStorageRequests { get; } = new();

    public FileUploadResponse UploadToPrivateStorageResponse { get; private set; } = CreateDefaultResponse();

    public void ConfigureUploadToPrivateStorageResponse(
        string? uploadId,
        string? url,
        string? contentType,
        int width,
        int height) =>
        UploadToPrivateStorageResponse = new FileUploadResponse
        {
            Id = string.IsNullOrWhiteSpace(uploadId) ? "fake-upload" : uploadId,
            Original = new GrpcFile
            {
                Url = string.IsNullOrWhiteSpace(url) ? "https://fake-core.local/private/fake-upload.pdf" : url,
                ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/pdf" : contentType,
                Width = width,
                Height = height,
            },
        };

    public IReadOnlyCollection<RecordedUploadToPrivateStorageRequest> SnapshotRecordedRequests(bool clearAfterRead)
    {
        var items = UploadToPrivateStorageRequests.ToArray();
        if (clearAfterRead)
        {
            ClearRecordedRequests();
        }

        return items;
    }

    public void ClearRecordedRequests()
    {
        while (UploadToPrivateStorageRequests.TryDequeue(out _))
        {
        }
    }

    public void Reset()
    {
        ClearRecordedRequests();
        UploadToPrivateStorageResponse = CreateDefaultResponse();
    }

    private static FileUploadResponse CreateDefaultResponse() =>
        new()
        {
            Id = "fake-upload",
            Original = new GrpcFile
            {
                Url = "https://fake-core.local/private/fake-upload.pdf",
                ContentType = "application/pdf",
            },
        };
}

public record RecordedUploadToPrivateStorageRequest(
    DateTimeOffset RequestedAtUtc,
    string? Extension,
    string? ContentType,
    byte[] Content)
{
    public int ContentLength => Content.Length;
}
