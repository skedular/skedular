using Api.Shared.Services.OpenApi.Skedular.Core.Core.V1;
using Core.Shared.Models;
using File = Api.Shared.Services.OpenApi.Skedular.Core.Core.V1.File;

namespace Core.Api.Mappers;

public interface IOpenApiMapper
{
    FileUploadResponse MapTo(CdnFile src);
    FileUploadResponse MapTo(PrivateFile src);
}

public class OpenApiMapper : IOpenApiMapper
{
    public FileUploadResponse MapTo(CdnFile src) =>
        new()
        {
            Id = src.Id,
            Original = new File { Url = src.CdnUrl.ToString(), ContentType = src.ContentType, Width = src.Width, Height = src.Height },
            Thumbnail = src.ThumbnailCdnUrl is null
                ? null
                : new File
                {
                    Url = src.ThumbnailCdnUrl.ToString(),
                    ContentType = src.ThumbnailContentType,
                    Width = src.ThumbnailWidth,
                    Height = src.ThumbnailHeight
                }
        };

    public FileUploadResponse MapTo(PrivateFile src) =>
        new()
        {
            Id = src.Id,
            Original = new File { Url = src.StorageUrl.ToString(), ContentType = src.ContentType, Width = src.Width, Height = src.Height },
            Thumbnail = src.ThumbnailStorageUrl is null
                ? null
                : new File
                {
                    Url = src.ThumbnailStorageUrl.ToString(),
                    ContentType = src.ThumbnailContentType,
                    Width = src.ThumbnailWidth,
                    Height = src.ThumbnailHeight
                }
        };
}
