using Api.Shared.Grpc.Skedular.Core.Core.V1;
using Core.Shared.Models;
using Enterprise.Shared;
using File = Api.Shared.Grpc.Skedular.Core.Core.V1.File;

namespace Core.Api.Mappers;

public interface IGrpcMapper
{
    FileUploadResponse MapToGrpcResponse(PrivateFile src);
}

public class GrpcMapper : IGrpcMapper
{
    public FileUploadResponse MapToGrpcResponse(PrivateFile src) =>
        new()
        {
            Id = src.Id.ToSafeString(),
            Original = new File
            {
                Url = src.StorageUrl.ToString().ToSafeString(),
                ContentType = src.ContentType.ToSafeString(),
                Width = src.Width.ToNullInt(),
                Height = src.Height.ToNullInt(),
            },
            Thumbnail = src.ThumbnailStorageUrl is null
                ? null
                : new File
                {
                    Url = src.ThumbnailStorageUrl.ToString().ToSafeString(),
                    ContentType = src.ThumbnailContentType.ToSafeString(),
                    Width = src.ThumbnailWidth.ToNullInt(),
                    Height = src.ThumbnailHeight.ToNullInt(),
                },
        };
}
