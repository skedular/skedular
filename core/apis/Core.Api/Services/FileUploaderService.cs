using Core.Api.Mappers;
using Core.Shared.Models;
using Core.Shared.Repositories;
using Enterprise.Shared.FileStorage;
using Enterprise.Shared.Image;
using Enterprise.Shared.Random;
using Customer = Core.Shared.Database.Entities.Customer;

namespace Core.Api.Services;

public interface IFileUploaderService
{
    Task<CdnFile> UploadToCdnAsync(
        Stream stream,
        string contentType,
        string? extension,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<PrivateFile> UploadToPrivateStorageAsync(
        Stream stream,
        string contentType,
        string? extension,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class FileUploaderService(
    ICustomerService customerService,
    ICdnService cdnService,
    IFileService fileService,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IMapper mapper,
    IImageHelper imageHelper) : IFileUploaderService
{
    public async Task<CdnFile> UploadToCdnAsync(
        Stream stream,
        string contentType,
        string? extension,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (_, customer) = await customerService.GetCustomerAsync(cancellationToken);
        }

        var id = randomHelper.Generate();
        var (storageUrl, cdnUrl) = await cdnService.UploadAsync(stream, contentType, id, extension, cancellationToken);
        var response = await imageHelper.GetImageWidthHeightAsync(stream, cancellationToken);
        var cdnFile = new Shared.Database.Entities.CdnFile
        {
            Id = id,
            StorageUrl = storageUrl.ToString(),
            CdnUrl = cdnUrl.ToString(),
            ContentType = contentType,
            Width = response.IsImage ? response.Width : null,
            Height = response.IsImage ? response.Height : null,
            UploadedBy = customer
        };

        if (response.IsImage)
        {
            var thumbnailResponse = await imageHelper.CreateThumbnailAsync(stream, cancellationToken);

            try
            {
                var (thumbnailStorageUrl, thumbnailCdnUrl) =
                    await cdnService.UploadAsync(thumbnailResponse.ThumbnailStream, contentType, $"{id}_thumbnail", ".png", cancellationToken);
                cdnFile.ThumbnailStorageUrl = thumbnailStorageUrl.ToString();
                cdnFile.ThumbnailCdnUrl = thumbnailCdnUrl.ToString();
                cdnFile.ThumbnailHeight = thumbnailResponse.Height;
                cdnFile.ThumbnailWidth = thumbnailResponse.Width;
                cdnFile.ThumbnailContentType = thumbnailResponse.ContentType;
            }
            finally
            {
                await thumbnailResponse.ThumbnailStream.DisposeAsync();
            }
        }

        repositoryFactory.CdnFileRepository.Add(cdnFile);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.MapTo(cdnFile);
    }

    public async Task<PrivateFile> UploadToPrivateStorageAsync(
        Stream stream,
        string contentType,
        string? extension,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (_, customer) = await customerService.GetCustomerAsync(cancellationToken);
        }

        var id = randomHelper.Generate();
        var storageUrl = await fileService.UploadAsync(stream, contentType, id, extension, cancellationToken);
        var response = await imageHelper.GetImageWidthHeightAsync(stream, cancellationToken);
        var privateFile = new Shared.Database.Entities.PrivateFile
        {
            Id = id,
            StorageUrl = storageUrl.ToString(),
            ContentType = contentType,
            Width = response.IsImage ? response.Width : null,
            Height = response.IsImage ? response.Height : null,
            UploadedBy = customer
        };

        if (response.IsImage)
        {
            var thumbnailResponse = await imageHelper.CreateThumbnailAsync(stream, cancellationToken);

            try
            {
                var thumbnailStorageUrl = await fileService.UploadAsync(
                    thumbnailResponse.ThumbnailStream,
                    contentType, $"{id}_thumbnail",
                    ".png",
                    cancellationToken);
                privateFile.ThumbnailStorageUrl = thumbnailStorageUrl.ToString();
                privateFile.ThumbnailHeight = thumbnailResponse.Height;
                privateFile.ThumbnailWidth = thumbnailResponse.Width;
                privateFile.ThumbnailContentType = thumbnailResponse.ContentType;
            }
            finally
            {
                await thumbnailResponse.ThumbnailStream.DisposeAsync();
            }
        }

        repositoryFactory.PrivateFileRepository.Add(privateFile);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.MapTo(privateFile);
    }
}
