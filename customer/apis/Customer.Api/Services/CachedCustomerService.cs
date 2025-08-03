using Api.Shared.Services;
using Api.Shared.Services.Cache;
using Customer.Api.Mappers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Context;

namespace Customer.Api.Services;

public interface ICachedCustomerService
{
    ValueTask<bool> DoesCustomerExistAsync(string verifiableToken, CancellationToken cancellationToken);
    ValueTask<Shared.Models.Customer> GetAsync(CancellationToken cancellationToken);
    ValueTask<Shared.Models.Customer?> GetNullableAsync(CancellationToken cancellationToken);
}

public class CachedCustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IContext context,
    IGenericCustomerCacheService genericCustomerCacheService)
    : ICachedCustomerService
{
    public async ValueTask<bool> DoesCustomerExistAsync(string verifiableToken, CancellationToken cancellationToken)
    {
        try
        {
            _ = await GetByVerifiableTokenAsync(verifiableToken, cancellationToken);
            return true;
        }
        catch (CustomerNotFound)
        {
            return false;
        }
    }

    public async ValueTask<Shared.Models.Customer> GetAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.GetVerifiableToken());

        return await GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
    }

    public async ValueTask<Shared.Models.Customer?> GetNullableAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.GetVerifiableToken()))
        {
            return null;
        }

        try
        {
            return await GetByVerifiableTokenAsync(context.GetVerifiableToken(), cancellationToken);
        }
        catch (CustomerNotFound)
        {
            return null;
        }
    }

    private async ValueTask<Shared.Models.Customer> GetByVerifiableTokenAsync(
        string verifiableToken,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);

        return await genericCustomerCacheService.GetByVerifiableTokenAsync(
            verifiableToken,
            async ct =>
            {
                var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, ct) ??
                               throw new CustomerNotFound();

                return mapper.MapTo(customer);
            },
            cancellationToken);
    }
}
