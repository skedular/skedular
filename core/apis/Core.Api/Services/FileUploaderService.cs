using Core.Shared.Database.Entities;
using Core.Shared.Repositories;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Random;

namespace Core.Api.Services;

public interface IFileUploaderService
{
    Task<(string, Uri)> UploadAsync(Stream stream, string contentType, string? extension, CancellationToken cancellationToken);
}

public class FileUploaderService(
    ICustomerService customerService,
    ICdnService cdnService,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper) : IFileUploaderService
{
    public async Task<(string, Uri)> UploadAsync(Stream stream, string contentType, string? extension, CancellationToken cancellationToken)
    {
        var (_, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);

        var id = randomHelper.Generate();
        var (storageUrl, cdnUrl) = await cdnService.UploadAsync(stream, contentType, id, extension, cancellationToken);
        var cdnFile = new CdnFile { Id = id, StorageUrl = storageUrl.ToString(), CdnUrl = cdnUrl.ToString(), UploadedBy = customerEntity };

        repositoryFactory.CdnFileRepository.Add(cdnFile);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return (id, cdnUrl);
    }
}
