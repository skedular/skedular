using Customer.Api.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;

namespace Customer.Api.Services;

public interface IWorkaroundService
{
    Task RepublishCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task RepublishAllCustomersAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, IMapper mapper, ICustomerPublisher customerPublisher) : IWorkaroundService
{
    public async Task RepublishCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
        {
            return;
        }

        await customerPublisher.PublishCustomerAsync([mapper.MapTo(customer)], cancellationToken);
    }

    public async Task RepublishAllCustomersAsync(CancellationToken cancellationToken)
    {
        var customers = await repositoryFactory.CustomerRepository.GetAllAsync(cancellationToken);
        await customerPublisher.PublishCustomerAsync(customers.Select(mapper.MapTo), cancellationToken);
    }
}
