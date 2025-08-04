using Api.Shared.Services;
using Api.Shared.Services.Cache;
using Core.Api.Mappers;
using Core.Shared.Models;
using Core.Shared.Repositories;
using Enterprise.Shared.Context;

namespace Core.Api.Services;

public interface ICachedCustomerService
{
    ValueTask<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
}

public class CachedCustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IContext context,
    IGenericCustomerCacheService genericCustomerCacheService)
    : ICachedCustomerService
{
    public async ValueTask<bool> DoesCustomerExistAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        try
        {
            _ = await GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
            return true;
        }
        catch (CustomerNotFound)
        {
            return false;
        }
    }

    private async ValueTask<Customer> GetByVerifiableTokenAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await genericCustomerCacheService.GetByVerifiableTokenAsync(
            verifiableToken,
            async ct =>
            {
                var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, ct) ??
                               throw new CustomerNotFound();

                return mapper.MapTo(customer)!;
            },
            cancellationToken);
    }
}
