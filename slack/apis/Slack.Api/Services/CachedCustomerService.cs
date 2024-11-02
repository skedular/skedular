using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Slack.Api.Mappers;
using Slack.Shared.Models;
using Slack.Shared.Repositories;

namespace Slack.Api.Services;

public interface ICachedCustomerService
{
    Task<bool> DoesCustomerExistAsync(CancellationToken cancellationToken);
    Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(CancellationToken cancellationToken);

    Task<(Customer, Shared.Database.Entities.Customer)>
        GetCustomerAsync(string id, CancellationToken cancellationToken);
}

public class CachedCustomerService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IContext context,
    IMemoryCache memoryCache) : ICachedCustomerService
{
    public async Task<bool> DoesCustomerExistAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        return await memoryCache.GetOrCreateAsync(
            $"customer-exists-{context.PropertyBag.VerifiableToken}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                return await repositoryFactory.CustomerRepository.Query(
                    new Specification<Shared.Database.Entities.Customer>
                    {
                        Criteria = query => !query.DeletedAt.HasValue && query.Identities
                            .Select(identity => identity.Id)
                            .Contains(context.PropertyBag.VerifiableToken)
                    }).AsNoTracking().AnyAsync(cancellationToken);
            });
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);

        return await memoryCache.GetOrCreateAsync(
            $"customer-verifiabletoken-{context.PropertyBag.VerifiableToken}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                var customer =
                    await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(
                        context.PropertyBag.VerifiableToken,
                        cancellationToken);
                if (customer is null)
                {
                    throw new CustomerNotFound();
                }

                return (mapper.MapTo(customer)!, customer);
            });
    }

    public async Task<(Customer, Shared.Database.Entities.Customer)> GetCustomerAsync(
        string id,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        return await memoryCache.GetOrCreateAsync(
            $"customer-id-{id}",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(id, cancellationToken);
                if (customer is null)
                {
                    throw new CustomerNotFound();
                }

                return (mapper.MapTo(customer)!, customer);
            });
    }
}
