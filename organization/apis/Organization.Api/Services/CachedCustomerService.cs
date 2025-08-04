using Api.Shared.Services;
using Api.Shared.Services.Cache;
using Enterprise.Shared.Context;
using Organization.Api.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface ICachedCustomerService
{
    ValueTask<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
    ValueTask<Customer> GetAsync(CancellationToken cancellationToken);
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

    public async ValueTask<Customer> GetAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        return await GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
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
