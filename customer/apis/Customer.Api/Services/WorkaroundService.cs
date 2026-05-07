using Customer.Shared.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;

namespace Customer.Api.Services;

public interface IWorkaroundService
{
    Task RepublishCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task RepublishAllCustomersAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, IEntityMapper entityMapper, ICustomerPublisher customerPublisher)
    : IWorkaroundService
{
    public async Task RepublishCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
        {
            return;
        }

        await customerPublisher.PublishCustomersAsync([entityMapper.MapTo(customer)], cancellationToken);
    }

    public async Task RepublishAllCustomersAsync(CancellationToken cancellationToken)
    {
        var customers = await repositoryFactory.CustomerRepository.GetAllUntrackedAsync(cancellationToken);
        await customerPublisher.PublishCustomersAsync(customers.Select(entityMapper.MapTo).ToList(), cancellationToken);
    }
}
