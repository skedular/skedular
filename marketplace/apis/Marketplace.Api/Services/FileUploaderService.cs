using Enterprise.Shared.Cdn;
using Enterprise.Shared.Random;
using Marketplace.Shared.Database.Entities;
using Marketplace.Shared.Repositories;

namespace Marketplace.Api.Services;

public interface IFileUploaderService
{
    Task<string> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken);
}

public class FileUploaderServiceService(
    ICdnService cdnService,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper) : IFileUploaderService
{
    public async Task<string> UploadAsync(Stream stream, string contentType, CancellationToken cancellationToken)
    {
        var id = randomHelper.Generate();
        var cdnFile = new CdnFile { Id = id, Url = await cdnService.UploadAsync(stream, contentType, id, cancellationToken) };

        repositoryFactory.CdnFileRepository.Add(cdnFile);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return cdnFile.Id;
    }
}
