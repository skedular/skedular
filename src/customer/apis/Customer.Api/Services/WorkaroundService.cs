using Customer.Shared.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;

namespace Customer.Api.Services;

public interface IWorkaroundService
{
    Task RepublishCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task RepublishAllCustomersAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(
    ILogger<WorkaroundService> logger,
    IRepositoryFactory repositoryFactory,
    IEntityMapper entityMapper,
    ICustomerPublisher customerPublisher)
    : IWorkaroundService
{
    public async Task RepublishCustomerAsync(string customerId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Backfill: republishing customer {CustomerId}", customerId);

        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
        {
            logger.LogWarning("Backfill: customer {CustomerId} not found, skipping republish", customerId);
            return;
        }

        await customerPublisher.PublishCustomersAsync([entityMapper.MapTo(customer)], cancellationToken);

        logger.LogInformation("Backfill: republish dispatched for customer {CustomerId}", customerId);
    }

    public async Task RepublishAllCustomersAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Backfill: republishing all customers");

        var customers = await repositoryFactory.CustomerRepository.GetAllUntrackedAsync(cancellationToken);
        await customerPublisher.PublishCustomersAsync([.. customers.Select(entityMapper.MapTo)], cancellationToken);

        logger.LogInformation("Backfill: republish dispatched for {CustomerCount} customers", customers.Count);
    }
}
