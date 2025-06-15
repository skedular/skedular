using Core.Api.Mappers;
using Core.Shared.Models;
using Core.Shared.Repositories;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Image;
using Enterprise.Shared.Random;

namespace Core.Api.Services;

public interface IFileUploaderService
{
    Task<CdnFile> UploadAsync(Stream stream, string contentType, string? extension, CancellationToken cancellationToken);
}

public class FileUploaderService(
    ICustomerService customerService,
    ICdnService cdnService,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IMapper mapper,
    IImageHelper imageHelper) : IFileUploaderService
{
    public async Task<CdnFile> UploadAsync(Stream stream, string contentType, string? extension, CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var id = randomHelper.Generate();
        var (storageUrl, cdnUrl) = await cdnService.UploadAsync(stream, contentType, id, extension, cancellationToken);

        stream.Position = 0;
        var response = await imageHelper.GetImageWidthHeightAsync(stream, cancellationToken);

        var cdnFile = new Shared.Database.Entities.CdnFile
        {
            Id = id,
            StorageUrl = storageUrl.ToString(),
            CdnUrl = cdnUrl.ToString(),
            ContentType = contentType,
            Width = response.IsImage ? response.Width : null,
            Height = response.IsImage ? response.Height : null,
            UploadedBy = customerEntity
        };

        repositoryFactory.CdnFileRepository.Add(cdnFile);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return mapper.MapTo(cdnFile);
    }
}
